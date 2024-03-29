# 股票分析與回測網頁應用
**Demo**: https://azuretestmvc.azurewebsites.net/Home/

這款網頁應用是一個全面的股票分析與回測平台，使用.Net Core 7 MVC建立。

它透過Entity Framework Core (EF Core) 與MySQL資料庫整合，有效地查詢和管理股票資料。

此應用提供了強大的功能，用於根據各種條件過濾股票，並進行多只股票的回測，以評估過去的表現和回報。

應用部署在Azure Web Services上，並利用Google Cloud MySQL，展示了在雲端部署、CI/CD流程以及網頁開發技能的熟練度。

**備註** : 由於此專案用於練習用途，股票資料只抓取台股市值前50大及部分ETF資料，資料區間為2010/01/01~2024/01/25。

## 特色功能
- **股票過濾**: 使用者可以設定多重條件來過濾股票。這項功能讓投資者能夠輕易地篩選出符合特定條件的股票，更容易識別潛在的投資機會。
- **回測功能**: 提供對多只股票在指定時間段內的回測能力。這幫助使用者評估股票的歷史表現，做出更有根據的投資決策。

## 技術棧
- **前端**: 使用.Net Core 7 MVC動態生成網頁，HTML、CSS、JavaScript進行客戶端腳本編寫。
- **後端**: 使用.Net Core 7進行服務端邏輯，Entity Framework Core進行資料庫操作。
- **資料庫**: 使用Google Cloud上的MySQL，利用雲端基礎的關係資料庫服務進行資料存儲和管理。
- **部署**: 使用Azure Web Services托管網頁應用，整合CI/CD管道進行自動部署和更新。

## 開始使用
**Demo**: https://azuretestmvc.azurewebsites.net/Home/
### 個股每日線圖
![image](https://github.com/SkyKai1018/azuretest/assets/135136212/1b48a3ad-e3c4-4c54-bd5c-f317642daca9)
### 個股篩選
![image](https://github.com/SkyKai1018/azuretest/assets/135136212/a032fb11-d477-4356-a673-3bd2c545fd07)
### 回測功能
![image](https://github.com/SkyKai1018/azuretest/assets/135136212/39332c48-bdcd-4c26-9e45-bafdddf2bebc)

### 使用教學
1. **HomePage**: 進入輸入欄位輸入股票代號，系統先找尋資料庫中符合的資料並顯示選項，當選擇股票後，會在下方圖表顯示該檔股票的每日線圖。
2. **股票篩選**: 使用者於左側篩選策略設定條件，按下加入會在右方顯示，可同時加入多個篩選策略，當按下開始篩選按鈕時，系統會將符合所有篩選條件的資料顯示在下方圖表。
3. **回測功能**: 提供最多三組回測比較的股票，可設定回測的時間區間，目前區間為2010/01~2024/01/25，另外可設定每月買入日期，當按下計算按鈕時，系統會每月持續買入100元，並將計算出投報率、買入次數等結果顯示在下方欄位，未來將開放初次投入、每次每日金額、股利計算等功能。
