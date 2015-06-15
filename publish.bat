@echo off
set BIDDINGDIR="..\bidding exe"
if exist %BIDDINGDIR% rd %BIDDINGDIR%
if not exist %BIDDINGDIR% md %BIDDINGDIR%
xcopy /s /e /i /y Accounting\Accounting\bin\Debug %BIDDINGDIR%\Accounting 
xcopy /s /e /i /y BidderDataInput\exe %BIDDINGDIR%\BidderDataInput
xcopy /s /e /i /y Checkout\Checkout\bin\Debug %BIDDINGDIR%\Checkout
xcopy /s /e /i /y DealerDataInput\exe %BIDDINGDIR%\DealerDataInput
xcopy /s /e /i /y SetAuction\SetAuction\bin\Debug %BIDDINGDIR%\SetAuction
xcopy /s /e /i /y Bidding\Bidding\bin\Debug %BIDDINGDIR%"\Bidding"
pause