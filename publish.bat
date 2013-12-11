@echo off
set BIDDINGDIR="..\bidding exe"
if not exist %BIDDINGDIR% md %BIDDINGDIR%
xcopy /s /e /i Accounting\Accounting\bin\Debug %BIDDINGDIR%\Accounting 
xcopy /s /e /i BidderDataInput\exe %BIDDINGDIR%\BidderDataInput
xcopy /s /e /i Checkout\Checkout\bin\Debug %BIDDINGDIR%\Checkout
xcopy /s /e /i DealerDataInput\exe %BIDDINGDIR%\DealerDataInput
xcopy /s /e /i SetAuction\SetAuction\bin\Debug %BIDDINGDIR%\SetAuction
xcopy /s /e /i "SJ Bidding System\SJ Bidding System\bin\Debug" %BIDDINGDIR%"\SJ Bidding System"
goto :eof