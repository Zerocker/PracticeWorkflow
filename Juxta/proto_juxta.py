# -*- coding: utf-8 -*-
#!/usr/bin/env python

from datetime import datetime
from fuzzywuzzy import fuzz

import multiprocessing as mp
import pandas as pd
import numpy as np
import argparse
import time

def reformat_date(date_str):
    dt_obj = datetime.strptime(date_str, '%Y-%m-%d %H:%M:%S')
    return dt_obj.strftime('%d.%m.%y')

parser = argparse.ArgumentParser(description="Автоматическое заполнение внутренних рейсов")
parser.add_argument('--new', dest='new_data', type=str, required=True,
                        help='Путь к файлу с новыми данными')
parser.add_argument('--in', dest='in_file', type=str, required=True,
                        help="Путь к основному файлу-списку")
parser.add_argument('--out', dest='out_file', type=str,
                        help='Путь к результирующему файлу')
args = parser.parse_args()

srch_data = pd.read_excel(args.new_data, dtype=str)
srch_data = srch_data.replace(np.nan, '', regex=True)

srch_data['Фамилия'] = srch_data[['Фамилия', 'Имя']].agg(' '.join, axis=1)
srch_sel = srch_data.loc[:,['Фамилия', 'Результат', 'ДО']]
print(srch_sel)
print('-' * 80, end="\n")

main_data = pd.read_excel(args.in_file, dtype=str)
main_data = main_data.replace(np.nan, '', regex=True)

input('*** Нажмите Enter для обработки ***')
print('-' * 80, end="\n")

def process(find_name, find_res, date_bypass, is_direct):
    for i, j in main_data.iterrows():
        main_name = j.iloc[4]
        main_date = j.iloc[1]
        main_resl = j.iloc[0]

        main_check = fuzz.token_sort_ratio(find_name.lower(), main_name.lower()) > 70 and \
            fuzz.token_set_ratio(find_name.lower(), main_name.lower()) > 80

        direct_check = (find_name.lower() in main_name.lower()) if is_direct else False

        if main_check or direct_check:

            if not date_bypass in main_date.lower():
                main_date += ', ' + date_bypass

            if (main_date.count(date_bypass) >= 2):
                words = main_date.split()
                main_date = " ".join(sorted(set(words), key=words.index))

            words = ['соблюд', 'самоизол', 'находи', 'прожив']
            for w in words:
                if w in find_res.lower():
                    main_resl = 'Соблюдает карантин'
                    break

            if ('двер' in find_res.lower()):
                main_resl = 'Дверь не открыли'
            elif ('местонахож' in find_res.lower() or 'устанавл' in find_res.lower() or 'не прожив' in find_res.lower()):
                main_resl = 'Место жительства устанавливается'
            elif ('госпитал' in find_res.lower()):
                main_resl = 'Госпитализация'

            if ('двер' in find_res.lower() and 'соблюд' in main_resl):
                main_date += '-д.н.о.'
            elif ('cоблюд' in find_res.lower() and 'двер' in main_resl):
                main_date += '-с.к.'

            main_data.iloc[i, 0] = main_resl
            main_data.iloc[i, 1] = main_date
            return f'{main_name[:25]:>25} | {main_resl[:25]:>25} | {main_date[-25:]:>25}'
    return ""

rows = list(zip(srch_sel['Фамилия'], srch_sel['Результат'], srch_sel['ДО']))
empty = []

for i, (name, res, date) in enumerate(rows):
    str_date = reformat_date(date)
    result = process(name, res, str_date, False)

    if (result):
        print(i+1, "|", result)
    else:
        empty += [i]
print('-' * 80, end="\n")

left = []
for idx in empty:
    err_name, err_res, err_date = rows[idx]
    err_date = reformat_date(err_date)

    result = process(err_name, err_res, err_date, True)
    if (result):
        print(idx+1, '|', result)
    else:
        left += [idx+1]
print('-' * 80, end="\n")
print(*left)
print(">", len(left))
print('-' * 80, end="\n")

main_data.to_excel(args.out_file)
print('Результат сохранен в', args.out_file)
