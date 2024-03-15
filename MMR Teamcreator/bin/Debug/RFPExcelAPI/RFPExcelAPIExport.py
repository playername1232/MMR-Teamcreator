import os, sys, json, argparse
import pygsheets
import pandas as pd
import numpy as np

currentDir = os.getcwd()

print("Connecting to google sheet...")
gcloud = pygsheets.authorize(service_account_file=f'{currentDir}\RFPExcelAPI\client_info.json', local=True)
gsheet = gcloud.open('ROG For People')
print("Connected...")

print("Connecting to 'tutodelame'")
try:
    worksheet = gsheet.worksheet_by_title('tutodelame')
except:
    worksheet = gsheet.add_worksheet('tutodelame')
print("Connected to 'tutodelame'")

print("Reading file...")
playerfile = open(f'{currentDir}\RFPExcelAPI\RawPlayerDataExport.txt', "r", encoding="utf-8")
print("File read.")
data = playerfile.read().split('\n')

counter = 0
skip = 0

print("Uploading data..")
while counter < 16:   
    _innerData = [data[0+(counter * 5)].split(':'), data[1+(counter * 5)].split(':'), data[2+(counter * 5)].split(':'), data[3+(counter * 5)].split(':'), data[4+(counter * 5)].split(':')]
    arr = np.array(_innerData)
    df = pd.DataFrame(arr, columns = ['Column_A','Column_B','Column_C'])
    worksheet.set_dataframe(df, (4 + (7*counter), 2), copy_head=False)

    counter += 1

print("Data uploaded successfully")