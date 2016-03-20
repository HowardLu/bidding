#-*- coding: cp950 -*-

"""
__version__ = "$Revision: 1.3 $"
__date__ = "$Date: 2004/04/14 02:38:47 $"
"""

from PythonCard import dialog, model
import pymongo, sys, re
import xlwt, xlrd

# 買家所有屬性
BIDDER_ATTRS = [	"BidderID", "Name", "Company", "CareerTitle", "IDNumber", "Tel", "Fax", "Address", "EMail", "Bank", "BankAcc",
									"BankContact", "BankContactTel", "CreditCardID", "CreditCardType", "Auctioneer", "GuaranteeCost", "ServiceFee"
								]

ALL_BIDDER_ATTRS = BIDDER_ATTRS + [ "GuaranteeType" ]

BIDDER_NONEMPTY_ATTRS = [ "BidderID", "Name", "IDNumber", "Tel", "Auctioneer" ]

# 買家資料輸出路徑
EXPORT_PATH	= "bidder_export.xls"

# 連線port
DB_CFG_PORT	= 27017

# 預設支付方式索引
GUARANTEE_INIT	= 0

# 寫死的login account
# DB_LOGIN_ACC = { "showmethemoney": None, "superjunior": "S", "igsigs": "N" }
DB_LOGIN_ACC = { "": None, "superjunior": "S", "igsigs": "N" }

# 無效牌號
INVALID_ID	= -1

NONEMPTY_ATTRS = BIDDER_NONEMPTY_ATTRS

# 暫存的IP存檔路徑
CACHED_IP_PATH	= "cached_ip.ini"

# 預設服務費%
DEFAULT_SERVICE_FEE_P = "20"

# 所有常數字串
Const			= {	"STATICTEXTNAME":						u"準買家姓名",
							"STATICTEXTCOMPANY":				u"公司名稱",
							"STATICTEXTCAREERTITLE":		u"職稱",
							"STATICTEXTIDNUMBER":				u"身分證/護照號碼",
							"STATICTEXTTEL":						u"電話",
							"STATICTEXTFAX":						u"傳真",
							"STATICTEXTADDRESS":				u"地址",
							"STATICTEXTEMAIL":					u"E-Mail",
							"STATICTEXTBANK":						u"往來銀行/分行",
							"STATICTEXTBANKACC":				u"帳號",
							"STATICTEXTAUCTIONEER":			u"拍賣商",
							"STATICTEXTBANKCONTACT":		u"銀行連絡人",
							"STATICTEXTBANKCONTACTTEL":	u"連絡電話",
							"STATICTEXTCREDITCARDID":		u"信用卡號碼",
							"STATICTEXTCREDITCARDTYPE":	u"信用卡類別",
							"STATICTEXTBIDDERID":				u"牌號",
							"STATICTEXTGUARANTEETYPE":	u"保證金繳納",
							"STATICTEXTGUARANTEECOST":	u"金額",
							"STATICTEXTSERVICEFEE":			u"服務費",
							"STATICTEXTSERVICEFEEP":		u"%",
							"BUTTONTEXTNEWFILE":				u"新建檔案",
							"BUTTONTEXTOPEN":						u"匯入檔案",
							"BUTTONTEXTCONNECT":				u"建立連線",
							"BUTTONTEXTSENDSINGLE":			u"單筆送出",
							"BUTTONTEXTSENDALL":				u"全部送出",
							"BUTTONTEXTADDBIDDER":			u"新增買家",
							"BUTTONTEXTFIXBIDDER":			u"修正資料",
							"BUTTONTEXTDELBIDDER":			u"刪除買家",
							"BUTTONTEXTCLEAR":					u"清空欄位",
							"BUTTONTEXTSEARCH":					u"搜尋/刷新",
							"BUTTONTEXTEXPORT":					u"匯出",
							"BUTTONTEXTIMPORT":					u"匯入",
							# "COMBOBOXGUARANTEETYPE":		u"支付方式",
							"COMBOBOXGUARANTEE_LIST":		[ u"台幣現鈔", u"日幣現鈔", u"人民幣現鈔", u"美金現鈔", u"信用卡", u"銀聯卡", u"VIP" ],
							"NEW_FILE_TITLE":						u"請建立一個資料檔",
							"FILE_RULE":								u"買家資料檔(*.txt)|*.txt",
							"NEW_FILE_FAILED":					u"你取消了資料檔的建立",
							"NEW_FILE_SUCCESS":					u"成功建立了新的資料檔，路徑為%s",
							"FILE_NOT_IMPORT":					u"<<你尚未與資料庫建立連線>>",
							"OPEN_FILE_TITLE":					u"請開啟一個資料檔",
							"OPEN_FILE_FAILED":					u"你取消了資料檔的開啟",
							"OPEN_FILE_SUCCESS":				u"成功匯入資料檔，路徑為%s",
							"OPEN_FILE_NAME":						u"資料檔<%s>",
							"TEXT_CHK_FAIL_EMPTY":			u"[%s]不可為空",
							"BIDDER_ID_LEN_EXCEEDED":		u"牌號位數過大，請修正",
							"BIDDER_ID_HAS_FOUR":				u"牌號不可含有4或是7",
							"BIDDER_ID_NOT_DIGIT":			u"牌號必須是數字，請修正",
							"GUARANTEECOST_NOT_DIGIT":	u"保證繳納金額必須是數字，請修正",
							"WARNING":									u"警告",
							"BIDDER_ID_REPEAT":					u"牌號已和[%s]重複",
							"ADD_BIDDER_SUCCESS":				u"成功新增了買家[%s]",
							"NO_SELECTED_BIDDER":				u"請選擇一個買家",
							"FIX_BIDDER_DONE":					u"已成功修改[%s]的資料",
							"NO_BIDDER_DATA":						u"查無買家資料!! ID = %d",
							"DEL_BIDDER_SUCCESS":				u"成功刪除了買家[%s]",
							"WIN_TITLE_TEXT":						u"競拍者資料建檔系統",
							"SAVE_FILE_SUCCESS":				u"資料檔已儲存",
							"MAX_DIGIT_NOW":						u"目前牌號最大位數為%d位",
							"UNSAVED_MODS_TITLE":				u"未儲存資料的處理方式",
							"UNSAVED_MODS_DENY":				u"你目前還有修改中的未儲存資料，無法進行此操作",
							"UNSAVED_MODS_QUESTION":		u"你目前還有修改中的未儲存資料，請問你接下來要....",
							"UNSAVED_MODS_QUIT":				u"不修改了，直接關閉",
							"UNSAVED_MODS_RESUME":			u"繼續編輯資料",
							"PLZ_ENTER_AUCTION_IP":			u"請輸入資料庫的IP位址",
							"PLZ_ENTER_LOGIN_CODE":			u"請輸入登入密碼",
							"READY_TO_CONNECT":					u"準備開始連線",
							"READY_TO_LOGIN":						u"登入中",
							"DEFAULT_IP":								u"127.0.0.1",
							"CONNECT_FAILED":						u"無法連線到[%s]",
							"CONNECT_SUCCESS":					u"成功連線到[%s]",
							"BIDDER_DATA_SENT":					u"已送出買家[%s]的完整資料至拍賣中心",
							"CONNECT_BEGIN":						u"正在連線至[%s]，請耐心等候",
							"ERRMSG_DB_CNCT_FAIL":			u"無法連線到資料庫( %s, %d )",
							"ERRMSG_DB_CNCT_OK":				u"成功連線到資料庫( %s, %d )",
							"ERRMSG_EXPORT_OK":					u"已匯出買家資料至""%s""",
						}

# 視窗表單物件
class MyBackground( model.Background ):
	# 物件初始化
	def on_initialize( self, event ):
		# 初始化所有文字 標題
		self.title = Const[ "WIN_TITLE_TEXT" ]
		
		# 靜態文字
		com = self.components
		com.StaticTextName.text						= Const[ "STATICTEXTNAME" ]
		com.StaticTextCompany.text				= Const[ "STATICTEXTCOMPANY" ]
		com.StaticTextCareerTitle.text		= Const[ "STATICTEXTCAREERTITLE" ]
		com.StaticTextIDNumber.text				= Const[ "STATICTEXTIDNUMBER" ]
		com.StaticTextTel.text						= Const[ "STATICTEXTTEL" ]
		com.StaticTextFax.text						= Const[ "STATICTEXTFAX" ]
		com.StaticTextAddress.text				= Const[ "STATICTEXTADDRESS" ]
		com.StaticTextEMail.text					= Const[ "STATICTEXTEMAIL" ]
		com.StaticTextBank.text						= Const[ "STATICTEXTBANK" ]
		com.StaticTextBankAcc.text				= Const[ "STATICTEXTBANKACC" ]
		com.StaticTextBankContact.text		= Const[ "STATICTEXTBANKCONTACT" ]
		com.StaticTextBankContactTel.text	= Const[ "STATICTEXTBANKCONTACTTEL" ]
		com.StaticTextCreditCardID.text		= Const[ "STATICTEXTCREDITCARDID" ]
		com.StaticTextCreditCardType.text	= Const[ "STATICTEXTCREDITCARDTYPE" ]
		com.StaticTextBidderID.text				= Const[ "STATICTEXTBIDDERID" ]
		com.StaticTextAuctioneer.text			= Const[ "STATICTEXTAUCTIONEER" ]
		com.StaticTextGuaranteeType.text	= Const[ "STATICTEXTGUARANTEETYPE" ]
		com.StaticTextGuaranteeCost.text	= Const[ "STATICTEXTGUARANTEECOST" ]
		com.StaticTextServiceFee.text			= Const[ "STATICTEXTSERVICEFEE" ]
		com.StaticTextServiceFeeP.text		= Const[ "STATICTEXTSERVICEFEEP" ]
		
		# 按鈕
		com.ButtonAddBidder.label					= Const[ "BUTTONTEXTADDBIDDER" ]
		com.ButtonFixBidder.label					= Const[ "BUTTONTEXTFIXBIDDER" ]
		com.ButtonDelBidder.label					= Const[ "BUTTONTEXTDELBIDDER" ]
		com.ButtonClear.label							= Const[ "BUTTONTEXTCLEAR" ]
		com.ButtonSearch.label						= Const[ "BUTTONTEXTSEARCH" ]
		com.ButtonExport.label						= Const[ "BUTTONTEXTEXPORT" ]
		com.ButtonImport.label						= Const[ "BUTTONTEXTIMPORT" ]
		
		# 選單
		com.ComboBoxGuaranteeType.text		= Const[ "COMBOBOXGUARANTEE_LIST" ][ GUARANTEE_INIT ]
		for type in Const[ "COMBOBOXGUARANTEE_LIST" ]:
			com.ComboBoxGuaranteeType.append( type )
		com.ComboBoxGuaranteeType.SetEditable( False )
		
		# 嘗試登入
		while True:
			login_result, self.__login_key = self.__login_process()
			if login_result:
				break
		
		if self.__login_key:
			com.TextFieldAuctioneer.text = self.__login_key
			com.TextFieldAuctioneer.SetEditable( False )
			com.TextFieldAuctioneer.enabled = False
			
		# 嘗試連上資料庫
		while True:
			connect_ip = self.__connection_process()
			if connect_ip:
				break
			
		self.__add_msg( Const[ "ERRMSG_DB_CNCT_OK" ] % ( connect_ip, DB_CFG_PORT ), True )
		
		# 取得資料庫相關資料表
		self.__mongo_db = self.__mongo_client.bidding_data
		self.__dbtable_buyer = self.__mongo_db.buyer_table
		
		# 直接顯示所有買家
		self.__gen_list_ui_by_bidder_data()
		
	# 讀取暫存的IP
	def __load_cached_ip( self ):
		try:
			file_ob = open( CACHED_IP_PATH, "r" )
			
		except:
			return "127.0.0.1"
		
		line = file_ob.readline()
		if line[ -1 ] == "\n":
			line = line[ : -1 ]
		
		file_ob.close()
		return line
		
	# 儲存暫存的IP
	def __save_cached_ip( self, connect_ip ):
		try:
			file_ob = open( CACHED_IP_PATH, "w" )
			
		except:
			return
		
		file_ob.write( connect_ip )
		file_ob.close()
		
	# 嘗試登入
	def __login_process( self ):
		result = dialog.textEntryDialog( self, Const[ "PLZ_ENTER_LOGIN_CODE" ], Const[ "READY_TO_LOGIN" ] )
		if not result.accepted:
			return False
		
		code = result.text
		
		if code not in DB_LOGIN_ACC:
			return False, False
		
		return True, DB_LOGIN_ACC[ code ]
		
	# 嘗試連上資料庫
	def __connection_process( self ):
		result = dialog.textEntryDialog( self, Const[ "PLZ_ENTER_AUCTION_IP" ], Const[ "READY_TO_CONNECT" ], self.__load_cached_ip() )
		if not result.accepted:
			return False
		
		connect_ip = result.text
		self.__save_cached_ip( connect_ip )
		
		try:
			self.__mongo_client = pymongo.MongoClient( connect_ip, DB_CFG_PORT )
		except:
			self.__add_msg( Const[ "ERRMSG_DB_CNCT_FAIL" ] % ( connect_ip, DB_CFG_PORT ), True )
			return False
			
		return connect_ip
		
	# 嘗試新增使用者
	def __db_add_buyer( self, bidder_data ):
		try:
			self.__dbtable_buyer.insert( bidder_data )
		except:
			self.__add_msg( Const[ "BIDDER_ID_REPEAT" ] % self.__gen_bid_id_plus_name_str( bidder_data[ "BidderID_int" ] ) )
			return 0
			
		return 1
		
	# 按下 新增使用者 按鈕
	def on_ButtonAddBidder_mouseClick( self, event ):
		# 如果無法取得資料 下面的function會送系統訊息
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return

		self.__add_bidder_by_data( bidder_data )
		
	# 藉由資料新增使用者
	def __add_bidder_by_data( self, bidder_data ):
		# 插入資料並重新整理頁面
		if not self.__db_add_buyer( bidder_data ):
			return
		
		self.__gen_list_ui_by_bidder_data()
		bidder_id = bidder_data[ "BidderID_int" ]
		self.__add_msg( Const[ "ADD_BIDDER_SUCCESS" ] % self.__gen_bid_id_plus_name_str( bidder_id ) )
		
		index, bidder_data = self.__cached_data[ bidder_id ]
		self.components.ListBidders.selection = index
		
	# 按下 編輯買家資料 按鈕
	def on_ButtonFixBidder_mouseClick( self, event ):
		# 沒有選取的項目
		index, bidder_id_ori = self.get_bidder_id_by_selection()
		if bidder_id_ori not in self.__cached_data:
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
			
		# 如果無法取得資料 下面的function會送系統訊息
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return
		
		bidder_id = bidder_data[ "BidderID_int" ]
		
		# 如果沒改牌號 那就是單純改資料
		search_key = { "BidderID_int": bidder_id_ori }
		if bidder_id_ori == bidder_id:
			self.__dbtable_buyer.update( search_key, bidder_data )
		
		# 如果要改牌號
		else:
			# 嘗試插入資料 如果失敗代表重複
			if not self.__db_add_buyer( bidder_data ):
				return
			
			# 刪除舊牌號資料
			self.__dbtable_buyer.remove( search_key )
			
		self.__gen_list_ui_by_bidder_data()
		self.__add_msg( Const[ "FIX_BIDDER_DONE" ] % self.__gen_bid_id_plus_name_str( bidder_id ) )
		
		index, bidder_data = self.__cached_data[ bidder_id ]
		self.components.ListBidders.selection = index
		
	# 按下 刪除買家資料 按鈕
	def on_ButtonDelBidder_mouseClick( self, event ):
		# 沒有選取的項目
		index, bidder_id = self.get_bidder_id_by_selection()
		if bidder_id not in self.__cached_data:
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# 取得要顯示的文字/index
		mag_arg = self.__gen_bid_id_plus_name_str( bidder_id );
		index, bidder_data = self.__cached_data[ bidder_id ]
		
		# 刪除牌號資料
		self.__dbtable_buyer.remove( { "BidderID_int": bidder_id } )
		
		# 刷新資料
		index_the_last = self.__gen_list_ui_by_bidder_data()
		self.__add_msg( Const[ "DEL_BIDDER_SUCCESS" ] % mag_arg )
		
		if index > index_the_last:
			index = index_the_last
		
		self.components.ListBidders.selection = index
		self.__display_bidder_data( {} )
		
	# 按下 清空資料 按鈕
	def on_ButtonClear_mouseClick( self, event ):
		self.__display_bidder_data( {} )
		
	# 按下 搜尋 按鈕
	def on_ButtonSearch_mouseClick( self, event ):
		search_text = self.components.TextFieldSearch.text
		search_text = None if search_text == "" else search_text
		self.__gen_list_ui_by_bidder_data( search_text )
		
	# 按下 匯出 按鈕
	def on_ButtonExport_mouseClick( self, event ):
		wb = xlwt.Workbook()
		ws = wb.add_sheet( "bidders" )
		
		com = self.components
		index_row = 0
		
		for index, attr in enumerate( ALL_BIDDER_ATTRS ):
			ws.write( index_row, index, getattr( com, "StaticText%s" % attr ).text )
		index_row += 1
		
		for cached_data_iter in self.__cached_data.itervalues():
			collection = cached_data_iter[ 1 ]
			
			for index, attr in enumerate( ALL_BIDDER_ATTRS ):
				ws.write( index_row, index, collection[ attr ] )
			
			index_row += 1
			
		wb.save( EXPORT_PATH )
		self.__add_msg( Const[ "ERRMSG_EXPORT_OK" ] % EXPORT_PATH )
	
	# 按下 匯入 按鈕
	def on_ButtonImport_mouseClick( self, event ):
		filter = "Office Excel Files|*.xls"
		result = dialog.fileDialog( self, 'Open', '', '', filter )
		
		if result.paths == None:
			return
		
		book = xlrd.open_workbook( result.paths[0] )
		sheet = book.sheet_by_index( 0 )
		
		for index_row in xrange( 1, sheet.nrows ):
			bidder_data = {}
			for index_col, attr in enumerate( ALL_BIDDER_ATTRS ):
				val = "%s" % sheet.cell( index_row, index_col ).value
				# self.__add_msg( "%s, %s" % ( val, type( val ) ) )
				bidder_data[ attr ] = val
				
			bidder_data = self.__gen_bidder_data_by_dict( bidder_data )
			if not bidder_data:
				continue

			self.__add_bidder_by_data( bidder_data )
		
		
	# 顯示買家資料
	def __display_bidder_data( self, bidder_data ):
		com = self.components
		for attr in BIDDER_ATTRS:
			getattr( com, "TextField" + attr ).text = bidder_data[ attr ] if attr in bidder_data else ""
			
		com.ComboBoxGuaranteeType.text = bidder_data[ "GuaranteeType" ] if "GuaranteeType" in bidder_data else ""
		if self.__login_key:
			com.TextFieldAuctioneer.text = self.__login_key
		
	# 按下買家列表資料
	def on_ListBidders_select( self, event = None ):
		index, bidder_id = self.get_bidder_id_by_selection()
		if bidder_id not in self.__cached_data:
			if bidder_id > 0:
				self.__add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return
		
		index, bidder_data = self.__cached_data[ bidder_id ]
		self.__display_bidder_data( bidder_data )
	
	# 利用索引值取得牌號ID
	def get_bidder_id_by_index( self, index ):
		if index in self.__chched_index:
			return self.__chched_index[ index ]
		
		return INVALID_ID
		
	# 利用介面選取取得牌號ID
	def get_bidder_id_by_selection( self ):
		index = self.components.ListBidders.selection
		if index < 0:
			return index, INVALID_ID
		
		return index, self.get_bidder_id_by_index( index )
	
	# 由介面的文字輸入格 產生買家資料
	def gen_bidder_data_by_text_field( self ):
		com = self.components
		
		bidder_data = {}
		for attr in BIDDER_ATTRS:
			chk_text = getattr( com, "TextField" + attr ).text
			bidder_data[ attr ] = chk_text
		
		bidder_data[ "GuaranteeType" ] = com.ComboBoxGuaranteeType.text
		
		return self.__gen_bidder_data_by_dict( bidder_data )
		
	# 由 Dict 產生買家資料
	def __gen_bidder_data_by_dict( self, bidder_data ):
		com = self.components
		
		# 部分資料不可為空
		for attr in BIDDER_ATTRS:
			chk_text = bidder_data[ attr ]
			if attr in NONEMPTY_ATTRS and not len( chk_text ):
				self.__add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % getattr( com, "StaticText" + attr ).text )
				return None
		
		bidder_id_str = bidder_data[ "BidderID" ]
		
		# 牌號過濾規則(不可有4, 7)
		# re_ob = re.compile( "[47]+" )
		# if re_ob.search( bidder_id_str ):
			# self.__add_msg( Const[ "BIDDER_ID_HAS_FOUR" ] )
			# return None
			
		# 牌號必須是純數字
		if not bidder_id_str.isdigit():
			self.__add_msg( Const[ "BIDDER_ID_NOT_DIGIT" ] )
			return None
		
		# 保證金繳納必須是純數字
		if bidder_data[ "GuaranteeCost" ] == "":
			com.TextFieldGuaranteeCost.text = "0"
			bidder_data[ "GuaranteeCost" ] = "0"
		
		if not bidder_data[ "GuaranteeCost" ].isdigit():
			self.__add_msg( Const[ "GUARANTEECOST_NOT_DIGIT" ] )
			return None
		
		# 服務費未填/亂填 一律採預設值
		service_fee = bidder_data[ "ServiceFee" ]
		if not service_fee.isdigit() or int( service_fee ) < 0 or int( service_fee ) > 100:
			bidder_data[ "ServiceFee" ] = DEFAULT_SERVICE_FEE_P
		
		# ID是整數
		bidder_data[ "BidderID_int" ] = int( bidder_data[ "BidderID" ] )
		
		# 建立PK
		bidder_data[ "_id" ] = bidder_data[ "BidderID_int" ]
		return bidder_data
		
	# 依買家資料產生界面顯示
	def __gen_list_ui_by_bidder_data( self, search_text = None ):
		com = self.components
		
		com.ListBidders.clear()
		
		self.__cached_data = {}
		self.__chched_index = {}
		index = -1
		
		search_key = {}
		if self.__login_key:
			search_key.update( { "Auctioneer": self.__login_key } )
		
		if search_text:
			if search_text.isdigit():
				extra_key = "BidderID"
			else:
				extra_key = "Name"
				
			search_key.update( { extra_key: re.compile( search_text ) } )
			
		if len( search_key ):
			find_result = self.__dbtable_buyer.find( search_key )
		else:
			find_result = self.__dbtable_buyer.find()
		
		for index, collection in enumerate( find_result.sort( "BidderID_int", 1 ) ):
			id = collection[ "BidderID_int" ]
			
			self.__cached_data[ id ] = index, collection
			self.__chched_index[ index ] = id
			com.ListBidders.append( self.__gen_bid_id_plus_name_str( id ) )
			
		return index
		
	# 產生牌號:姓名的字串
	def __gen_bid_id_plus_name_str( self, bidder_id ):
		index, bidder_data = self.__cached_data[ bidder_id ]
		return "%s: %s" % ( bidder_data[ "BidderID" ], bidder_data[ "Name" ] )
		
	# 程序紀錄
	def __add_msg( self, msg, plus_pop = False ):
		if plus_pop:
			dialog.alertDialog( self, msg, Const[ "WARNING" ] )
		self.components.TextAreaLog.appendText( msg + "\n" )

	# 任何文字更新(暴力法列出 suck!)
	def on_text_update( self, src_func ):
		self.saved_flag = False
	def on_TextFieldBidderID_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBidderID_textUpdate" )
	def on_TextFieldName_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldName_textUpdate" )
	def on_TextFieldCompany_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCompany_textUpdate" )
	def on_TextFieldCareerTitle_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCareerTitle_textUpdate" )
	def on_TextFieldIDNumber_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldIDNumber_textUpdate" )
	def on_TextFieldTel_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldTel_textUpdate" )
	def on_TextFieldFax_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldFax_textUpdate" )
	def on_TextFieldAddress_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldAddress_textUpdate" )
	def on_TextFieldEMail_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldEMail_textUpdate" )
	def on_TextFieldBank_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBank_textUpdate" )
	def on_TextFieldBankAcc_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBankAcc_textUpdate" )
	def on_TextFieldBankContact_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBankContact_textUpdate" )
	def on_TextFieldBankContactTel_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldBankContactTel_textUpdate" )
	def on_TextFieldCreditCardID_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCreditCardID_textUpdate" )
	def on_TextFieldCreditCardType_textUpdate( self, event ):
		self.on_text_update( "on_TextFieldCreditCardType_textUpdate" )
		
if __name__ == '__main__':
	app = model.Application(MyBackground)
	app.MainLoop()
