from asyncio.windows_events import NULL
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

print("Loading data...")
loaded = worksheet.get_values_batch(['D119:D198', 'E119:E198', 'F119:F198'] )
if loaded != NULL:
    print("Data loaded!")
try:
    test = loaded[0][79]
except:
    print("List doesn't contain 80 players!")
    exit(-1)

print("Writing into file!")
saveFile = open(f'{currentDir}\RFPExcelAPI\RawPlayerDataImport.txt', "w", encoding="utf-8")

i = 0
while i < 80:

    ingame = "".join(loaded[0][i])
    rank = "".join(loaded[1][i])
    twitch = "".join(loaded[2][i])

    if i != 80:
        saveFile.write("%s:%s:%s\n" % (ingame, rank, twitch))
    else:
        saveFile.write("%s:%s:%s" % (ingame, rank, twitch))

    i+=1

saveFile.close()
print("Writing completed!")