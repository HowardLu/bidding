#-*- coding: cp950 -*-

"""
__version__ = "$Revision: 1.3 $"
__date__ = "$Date: 2004/04/14 02:38:47 $"
"""

from PythonCard import dialog, model, EXIF, graphic
import pymongo, sys, time, gridfs
from bson.objectid import ObjectId

ATTR_MAP = {	# 賣家屬性
							"Dealer":
								{	"ALL_ATTR":
										(	"Name", "Country", "CardID", "ContractDate", "ContractID", "Tel", "CellPhone", "Fax", "Address", "BankName", "BankAcc", "PostID", "IfDealedInsuranceFee",
											"IfDealedServiceFee", "IfNDealedInsuranceFee", "IfNDealedServiceFee", "IfDealedPictureFee", "IfNDealedPictureFee", "FrameFee", "FireFee", "IdentifyFee"
										),
									"NONEMPTY_ATTR":
										( "Name", "ContractID", "Tel", "Country", "CardID"
										),
									"DIGIT_ATTR":
										( "IfDealedInsuranceFee", "IfDealedServiceFee", "IfNDealedInsuranceFee", "IfNDealedServiceFee", "IfDealedPictureFee", "IfNDealedPictureFee", "FrameFee", "FireFee", "IdentifyFee"
										),
									"KEY_ATTR": "Name", "ListCom": "ListDealer", "CacheData": { "Main": {}, "Index": {} }
								},
							# 賣家商品屬性
							"Item":
								{	"ALL_ATTR":
										( "ItemName", "ItemNum", "Spec", "ReservePrice", "Remain", "ItemPS", "LotNO"
										),
									"NONEMPTY_ATTR":
										( "ItemName"
										),
									"DIGIT_ATTR":
										( "ItemNum", "ReservePrice"
										),
									"KEY_ATTR": "_id", "ListCom": "ListItem", "CacheData": { "Main": {}, "Index": {} }
								},
						}

DEALER_KEY	= ATTR_MAP[ "Dealer" ][ "KEY_ATTR" ]
ITEM_KEY		= ATTR_MAP[ "Item" ][ "KEY_ATTR" ]

ITEM_COM_DATA = { "PackImg": "BitmapCanvasPack", "BuildImg": "BitmapCanvasBuild" }

# 連線port
CACHED_IP_PATH	= "cached_ip.ini"
DB_CFG_PORT			= 27017

# 無效Key
INVALID_KEY = ""

# 所有常數字串
Const			= {	"STATICTEXTNAME":												"委託方",
							"STATICTEXTCOUNTRY":										"國籍",
							"STATICTEXTCARDID":											"證件號碼",
							"STATICTEXTCONTRACTDATE":								"合約日期",
							"STATICTEXTCONTRACTID":									"合約編號",
							"STATICTEXTEL":													"電話",
							"STATICTEXTCELLPHONE":									"手機",
							"STATICTEXFAX":													"傳真",
							"STATICTEXTADDRESS":										"詳細地址",
							"STATICTEXTBANKNAME":										"開戶銀行",
							"STATICTEXTBANKACC":										"銀行帳戶",
							"STATICTEXTPOSTID":											"郵編",
							"STATICTEXTDEALERLIST":									"賣家列表",
							"STATICTEXTIFDEALED":										"如拍品成交：",
							"STATICTEXTIFNDEALED":									"如拍品未成交：",
							"STATICTEXTIFDEALEDINSURANCEFEE":				"保險費 = 落槌價之",
							"STATICTEXTIFDEALEDSERVICEFEE":					"服務費 = 落槌價之",
							"STATICTEXTIFNDEALEDINSURANCEFEE":			"保險費 = 保留價之",
							"STATICTEXTIFNDEALEDSERVICEFEE":				"服務費 = 保留價之",
							"STATICTEXTPICTUREFEE":									"圖錄費 =",
							"STATICTEXTOTHERFEE":										"其它費用：",
							"STATICTEXTFRAMEFEE":										"裝裱/鏡框/囊匣 =",
							"STATICTEXTFIREFEE":										"火      漆      費 =",
							"STATICTEXTIDENTIFYFEE":								"鑒      定      費 =",
							"STATICTEXTITEMLIST":										"委託拍賣標的列表",
							"BUTTONTEXTADD":												"新增",
							"BUTTONTEXTFIX":												"修正",
							"BUTTONTEXTDEL":												"刪除",
							"BUTTONTEXTSET":												"設定",
							"STATICTEXTPERCENT":										"%",
							"STATICTEXTSERIAL":											u"序號:%s",
							"STATICTEXTNTDPERITEM":									"NTD/件",
							"STATICTEXTITEMNAME":										"拍賣標的名稱",
							"STATICTEXTBUILDIMG":										"建檔圖片",
							"STATICTEXTPACKIMG":										"包裝圖片",
							"STATICTEXTITEMNUM":										"數量",
							"STATICTEXTSPEC":												"規格(cm)",
							"STATICTEXTRESERVEPRICE":								"保留價(NTD)",
							"STATICTEXTRESERVEPRICEPOST":						"萬",
							"STATICTEXTREMAIN":											"保存現狀",
							"STATICTEXTITEMPS":											"備註",
							"STATICTEXTLOTNO":											"拍品編號",
							"STATICTEXTNOTSET":											"未設定",
							"STATICTEXTDELIMAGETITLE":							u"是否確定將此張圖片的設定為空白",
							"STATICTEXTDEL":												u"設定為空白",
							"STATICTEXTKEEP":												u"保留設定",
							######################################
							"NEW_FILE_TITLE":												"請建立一個資料檔",
							"FILE_RULE":														"買家資料檔(*.txt)|*.txt",
							"NEW_FILE_FAILED":											"你取消了資料檔的建立",
							"NEW_FILE_SUCCESS":											u"成功建立了新的資料檔，路徑為%s",
							"FILE_NOT_IMPORT":											"<<你尚未與資料庫建立連線>>",
							"OPEN_FILE_TITLE":											"請開啟一張圖片",
							"OPEN_FILE_FAILED":											"你取消了資料檔的開啟",
							"OPEN_FILE_SUCCESS":										u"成功匯入資料檔，路徑為%s",
							"OPEN_FILE_NAME":												u"資料檔<%s>",
							"TEXT_CHK_FAIL_EMPTY":									u"[%s]不可為空",
							"TEXT_CHK_FAIL_MUST_DIGIT":							u"[%s]必須為數字",
							"TEXT_LOT_NO_NOT_EXIST":								u"拍品編號[%s]不存在",
							"TEXT_LOT_NO_REPEATED":									u"拍品編號[%s]已重複",
							"BIDDER_ID_LEN_EXCEEDED":								"牌號位數過大，請修正",
							"BIDDER_ID_HAS_FOUR":										"牌號不可含有""4""或是""7""",
							"BIDDER_ID_NOT_DIGIT":									"牌號必須是數字，請修正",
							"WARNING":															"警告",
							"ERRMSG_NAME_REPEAT":										u"不可有重複的名稱",
							"ADD_DEALER_SUCCESS":										u"成功新增了賣家[%s]",
							"ADD_ITEM_SUCCESS":											u"成功新增了拍品[%s]",
							"NO_SELECTED_BIDDER":										"請選擇一個買家",
							"NO_SELECTED_ITEM":											"請選擇一個拍品",
							"FIX_DATA_DONE":												u"已成功修改[%s]的資料",
							"NO_DEALER_DATA":												"查無買家資料!! ID = %d",
							"DEL_DATA_SUCCESS":											u"成功刪除了[%s]",
							"WIN_TITLE_TEXT":												"台灣世家拍賣-賣家資料建檔系統",
							"SAVE_FILE_SUCCESS":										"資料檔已儲存",
							"MAX_DIGIT_NOW":												"目前牌號最大位數為%d位",
							"UNSAVED_MODS_TITLE":										"未儲存資料的處理方式",
							"UNSAVED_MODS_DENY":										"你目前還有修改中的未儲存資料，無法進行此操作",
							"UNSAVED_MODS_QUESTION":								"你目前還有修改中的未儲存資料，請問你接下來要....",
							"UNSAVED_MODS_QUIT":										u"不修改了，直接關閉",
							"UNSAVED_MODS_RESUME":									"繼續編輯資料",
							"PLZ_ENTER_AUCTION_IP":									"請輸入拍賣中心的IP位址",
							"READY_TO_CONNECT":											"準備開始連線",
							"DEFAULT_IP":														"127.0.0.1",
							"CONNECT_FAILED":												"無法連線到[%s]",
							"CONNECT_SUCCESS":											"成功連線到[%s]",
							"BIDDER_DATA_SENT":											u"已送出買家[%s]的完整資料至拍賣中心",
							"CONNECT_BEGIN":												u"正在連線至[%s]，請耐心等候",
							"ERRMSG_DB_CNCT_FAIL":									u"無法連線到資料庫( %s, %d )",
							"ERRMSG_DB_CNCT_OK":										u"成功連線到資料庫( %s, %d )",
							"ERRMSG_INVALID_IMG_FILE":							u"這不是一張有效的圖片檔",
							"ERRMSG_IMG_ID_NOT_EXIST":							u"圖片資料庫ID[%s]不存在",
						}

# 視窗表單物件
class MyBackground( model.Background ):
	# 物件初始化
	def on_initialize( self, event ):
		# 初始化所有文字 標題
		self.title = Const[ "WIN_TITLE_TEXT" ]
		
		# 靜態文字
		com = self.components
		com.StaticTextName.text											= Const[ "STATICTEXTNAME" ]
		com.StaticTextCountry.text									= Const[ "STATICTEXTCOUNTRY" ]
		com.StaticTextCardID.text										= Const[ "STATICTEXTCARDID" ]
		com.StaticTextContractDate.text							= Const[ "STATICTEXTCONTRACTDATE" ]
		com.StaticTextContractID.text								= Const[ "STATICTEXTCONTRACTID" ]
		com.StaticTextTel.text											= Const[ "STATICTEXTEL" ]
		com.StaticTextCellPhone.text								= Const[ "STATICTEXTCELLPHONE" ]
		com.StaticTextFax.text											= Const[ "STATICTEXFAX" ]
		com.StaticTextAddress.text									= Const[ "STATICTEXTADDRESS" ]
		com.StaticTextBankName.text									= Const[ "STATICTEXTBANKNAME" ]
		com.StaticTextBankAcc.text									= Const[ "STATICTEXTBANKACC" ]
		com.StaticTextPostID.text										= Const[ "STATICTEXTPOSTID" ]
		com.StaticTextDealerList.text								= Const[ "STATICTEXTDEALERLIST" ]
		com.StaticTextIfDealed.text									= Const[ "STATICTEXTIFDEALED" ]
		com.StaticTextIfNDealed.text								= Const[ "STATICTEXTIFNDEALED" ]
		com.StaticTextIfDealedInsuranceFee.text			= Const[ "STATICTEXTIFDEALEDINSURANCEFEE" ]
		com.StaticTextIfDealedServiceFee.text				= Const[ "STATICTEXTIFDEALEDSERVICEFEE" ]
		com.StaticTextIfNDealedInsuranceFee.text		= Const[ "STATICTEXTIFNDEALEDINSURANCEFEE" ]
		com.StaticTextIfNDealedServiceFee.text			= Const[ "STATICTEXTIFNDEALEDSERVICEFEE" ]
		com.StaticTextIfDealedPictureFee.text				= Const[ "STATICTEXTPICTUREFEE" ]
		com.StaticTextIfNDealedPictureFee.text			= Const[ "STATICTEXTPICTUREFEE" ]
		com.StaticTextOtherFee.text									= Const[ "STATICTEXTOTHERFEE" ]
		com.StaticTextFrameFee.text									= Const[ "STATICTEXTFRAMEFEE" ]
		com.StaticTextFireFee.text									= Const[ "STATICTEXTFIREFEE" ]
		com.StaticTextIdentifyFee.text							= Const[ "STATICTEXTIDENTIFYFEE" ]
		com.StaticTextItemList.text									= Const[ "STATICTEXTITEMLIST" ]
		
		com.StaticTextPercent1.text									= Const[ "STATICTEXTPERCENT" ]
		com.StaticTextPercent2.text									= Const[ "STATICTEXTPERCENT" ]
		com.StaticTextPercent3.text									= Const[ "STATICTEXTPERCENT" ]
		com.StaticTextPercent4.text									= Const[ "STATICTEXTPERCENT" ]
		
		com.StaticTextNTDperItem1.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem2.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem3.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem4.text							= Const[ "STATICTEXTNTDPERITEM" ]
		com.StaticTextNTDperItem5.text							= Const[ "STATICTEXTNTDPERITEM" ]
		
		com.StaticTextItemName.text									= Const[ "STATICTEXTITEMNAME" ]
		com.StaticTextBuildImg.text									= Const[ "STATICTEXTBUILDIMG" ]
		com.StaticTextPackImg.text									= Const[ "STATICTEXTPACKIMG" ]
		
		com.StaticTextItemNum.text									= Const[ "STATICTEXTITEMNUM" ]
		com.StaticTextSpec.text											= Const[ "STATICTEXTSPEC" ]
		com.StaticTextReservePrice.text							= Const[ "STATICTEXTRESERVEPRICE" ]
		com.StaticTextReservePricePost.text					= Const[ "STATICTEXTRESERVEPRICEPOST" ]
		com.StaticTextRemain.text										= Const[ "STATICTEXTREMAIN" ]
		com.StaticTextItemPS.text										= Const[ "STATICTEXTITEMPS" ]
		com.StaticTextLotNO.text										= Const[ "STATICTEXTLOTNO" ]
		
		com.StaticTextPackImgState.text							= Const[ "STATICTEXTNOTSET" ]
		com.StaticTextBuildImgState.text						= Const[ "STATICTEXTNOTSET" ]
		
		com.StaticTextSerial.text										= ""
		
		# 按鈕
		com.ButtonAddDealer.label										= Const[ "BUTTONTEXTADD" ]
		com.ButtonFixDealer.label										= Const[ "BUTTONTEXTFIX" ]
		com.ButtonDelDealer.label										= Const[ "BUTTONTEXTDEL" ]
		com.ButtonAddItem.label											= Const[ "BUTTONTEXTADD" ]
		com.ButtonFixItem.label											= Const[ "BUTTONTEXTFIX" ]
		com.ButtonDelItem.label											= Const[ "BUTTONTEXTDEL" ]
		com.ButtonSetBuildImg.label									= Const[ "BUTTONTEXTSET" ]
		com.ButtonSetPackImg.label									= Const[ "BUTTONTEXTSET" ]
		
		# 圖片相關介面
		com.BitmapCanvasPack.visible = False
		com.BitmapCanvasBuild.visible = False
		
		# 嘗試連上資料庫
		while True:
			connect_ip = self.__connection_process()
			if connect_ip:
				break
			
		self.__add_msg( Const[ "ERRMSG_DB_CNCT_OK" ] % ( connect_ip, DB_CFG_PORT ), True )
		 
		# 取得資料庫相關資料表
		self.__mongo_db = self.__mongo_client.bidding_data
		self.__file_store = gridfs.GridFS( self.__mongo_db )
		# print dir( self.__file_store )
		
		self.__dbtable_main					= self.__mongo_db.dealer_table
		self.__dbtable_item					= self.__mongo_db.dealer_item_table
		self.__dbtable_item_serial	= self.__mongo_db.dealer_item_serail
		self.__dbtable_auction			= self.__mongo_db.auctions_table
		
		# 直接顯示所有賣家
		self.__gen_list_ui_by_data()
		
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
	def __add_dict( self, dict_data ):
		try:
			self.__dbtable_main.insert( dict_data )
		except:
			self.__add_msg( Const[ "ERRMSG_NAME_REPEAT" ], True )
			return 0
			
		return 1
		
	# 取得快取資料
	def __get_cache_data( self, type, sub_type ):
		return ATTR_MAP[ type ][ "CacheData" ][ sub_type ]
		
	# 按下 新增使用者 按鈕
	def on_ButtonAddDealer_mouseClick( self, event ):
		# 如果無法取得資料 下面的function會送系統訊息
		dict_data = self.__gen_data_by_text_field()
		if not dict_data:
			return

		# 插入資料並重新整理頁面
		if not self.__add_dict( dict_data ):
			return
		
		self.__gen_list_ui_by_data()
		self.__add_msg( Const[ "ADD_DEALER_SUCCESS" ] % self.__gen_dealer_title( dict_data ) )
		
		dealer_name = dict_data[ DEALER_KEY ]
		index, dict_data = self.__get_cache_data( "Dealer", "Main" )[ dealer_name ]
		self.components.ListDealer.selection = index
		
	# 顯示序號
	def __show_serail( self, dict_data ):
		self.components.StaticTextSerial.text = "" if "_id" not in dict_data else Const[ "STATICTEXTSERIAL" ] % dict_data[ "_id" ]
	
	# 按下 新增道具 按鈕
	def on_ButtonAddItem_mouseClick( self, event ):
		# 沒有選取的項目
		index, key = self.__get_key_by_selection( "Dealer" )
		if key not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# 如果無法取得資料 下面的function會送系統訊息
		dict_data = self.__gen_item_data_by_text_field( key, None )
		if not dict_data:
			return

		dict_data[ "_id" ] = self.__gen_item_serial()
		
		# 插入資料並重新整理頁面
		if not self.__add_item_dict( dict_data ):
			return
		
		self.__gen_item_list_ui_by_data( dict_data[ "SrcDealer" ] )
		self.__add_msg( Const[ "ADD_ITEM_SUCCESS" ] % self.__gen_item_title( dict_data ) )
		
		item_key = dict_data[ ITEM_KEY ]
		index, dict_data = self.__get_cache_data( "Item", "Main" )[ item_key ]
		self.__show_serail( dict_data )
		self.components.ListItem.selection = index
		
	# 按下 編輯買家資料 按鈕
	def on_ButtonFixDealer_mouseClick( self, event ):
		# 沒有選取的項目
		index, key_ori = self.__get_key_by_selection( "Dealer" )
		if key_ori not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
			
		# 如果無法取得資料 下面的function會送系統訊息
		dict_data = self.__gen_data_by_text_field()
		if not dict_data:
			return
		
		key = dict_data[ DEALER_KEY ]
		
		# 如果沒改Key 那就是單純改資料
		search_key = { DEALER_KEY: key_ori }
		if key_ori == key:
			self.__dbtable_main.update( search_key, dict_data )
		
		# 如果要改Key
		else:
			# 嘗試插入資料 如果失敗代表重複
			if not self.__add_dict( dict_data ):
				return
			
			# 刪除舊牌號資料
			self.__dbtable_main.remove( search_key )
			
		self.__gen_list_ui_by_data()
		self.__add_msg( Const[ "FIX_DATA_DONE" ] % self.__gen_dealer_title( dict_data ) )
		
		index, dict_data = self.__get_cache_data( "Dealer", "Main" )[ key ]
		self.components.ListDealer.selection = index
		
		
	# 按下 編輯拍品資料 按鈕
	def on_ButtonFixItem_mouseClick( self, event ):
		# 沒有選取的賣家
		index_src, key_src = self.__get_key_by_selection( "Dealer" )
		if key_src not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# 沒有選取的項目
		index, key = self.__get_key_by_selection( "Item" )
		if key not in self.__get_cache_data( "Item", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_ITEM" ] )
			return
		
		# 取得圖片ID 等等有可能要刪掉
		index, dict_data_ori = self.__get_cache_data( "Item", "Main" )[ key ]
		
		# 如果無法取得資料 下面的function會送系統訊息
		dict_data = self.__gen_item_data_by_text_field( key_src, key )
		if not dict_data:
			return
		
		# 已取得 資料可刪
		for pic_attr in ITEM_COM_DATA.iterkeys():
			self.__file_store.delete( dict_data_ori[ pic_attr ] )
			
		# 如果沒改Key 那就是單純改資料
		search_key = { ITEM_KEY: key }
		self.__dbtable_item.update( search_key, dict_data )
		
		self.__gen_item_list_ui_by_data( key_src )
		self.__add_msg( Const[ "FIX_DATA_DONE" ] % self.__gen_item_title( dict_data ) )
		
		index, dict_data = self.__get_cache_data( "Item", "Main" )[ key ]
		self.components.ListItem.selection = index
	
	# 按下 刪除賣家資料 按鈕
	def on_ButtonDelDealer_mouseClick( self, event ):
		# 沒有選取的項目
		index, key = self.__get_key_by_selection( "Dealer" )
		cache_data = self.__get_cache_data( "Dealer", "Main" )
		if key not in cache_data:
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# 取得要顯示的文字/index
		index, dict_data = cache_data[ key ]
		mag_arg = self.__gen_dealer_title( dict_data );
		
		# 刪除牌號資料
		self.__dbtable_main.remove( { DEALER_KEY: key } )
		self.__dbtable_item.remove( { "SrcDealer": key } )
		
		# 刷新資料
		index_the_last = self.__gen_list_ui_by_data()
		self.__add_msg( Const[ "DEL_DATA_SUCCESS" ] % mag_arg )
		
		if index > index_the_last:
			index = index_the_last
		
		self.components.ListDealer.selection = index
		self.on_ListDealer_select()
		
	# 按下 刪除拍品資料 按鈕
	def on_ButtonDelItem_mouseClick( self, event ):
		# 沒有選取的賣家
		index_src, key_src = self.__get_key_by_selection( "Dealer" )
		if key_src not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# 沒有選取的項目
		index, key = self.__get_key_by_selection( "Item" )
		cache_data = self.__get_cache_data( "Item", "Main" )
		if key not in cache_data:
			self.__add_msg( Const[ "NO_SELECTED_ITEM" ] )
			return
		
		# 取得要顯示的文字/index
		index, dict_data = cache_data[ key ]
		mag_arg = self.__gen_item_title( dict_data );
		
		# 先刪除圖片資料
		for pic_attr in ITEM_COM_DATA.iterkeys():
			self.__file_store.delete( dict_data[ pic_attr ] )
		
		# 刪除拍品資料
		self.__dbtable_item.remove( { ITEM_KEY: key } )
		
		# 刷新資料
		index_the_last = self.__gen_item_list_ui_by_data( key_src )
		self.__add_msg( Const[ "DEL_DATA_SUCCESS" ] % mag_arg )
		
		if index > index_the_last:
			index = index_the_last
		
		self.components.ListItem.selection = index
		self.on_ListItem_select()
		
	# 按下買家列表資料
	def on_ListDealer_select( self, event = None ):
		index, key = self.__get_key_by_selection( "Dealer" )
		cache_data = self.__get_cache_data( "Dealer", "Main" )
		if key not in cache_data:
			return
		
		index, dict_data = cache_data[ key ]
		attr_list = ATTR_MAP[ "Dealer" ][ "ALL_ATTR" ]
		for attr in attr_list:
			value = dict_data[ attr ] if attr in dict_data else ""
			getattr( self.components, "TextField" + attr ).text = value
			
		self.__gen_item_list_ui_by_data( key )
		self.on_ListItem_select()
	
	# 取得暫存圖檔檔名
	def __get_temp_bmp_fname( self, keyword ):
		return "tmp_pic_%s" % keyword
		
	# 按下拍品列表資料
	def on_ListItem_select( self, event = None ):
		index, key = self.__get_key_by_selection( "Item" )
		cache_data = self.__get_cache_data( "Item", "Main" )
		if key not in cache_data:
			dict_data = {}
		else:
			index, dict_data = cache_data[ key ]
		
		# 文字部分
		com = self.components
		attr_list = ATTR_MAP[ "Item" ][ "ALL_ATTR" ]
		for attr in attr_list:
			getattr( com, "TextField" + attr ).text = dict_data[ attr ] if attr in dict_data else ""
			
		# 圖片部分
		for pic_attr in ITEM_COM_DATA.iterkeys():
			pic_loaded = False
			if pic_attr in dict_data and dict_data[ pic_attr ]:
				fname = "tmp_pic_%s.jpg" % pic_attr
				file_obj = open( fname, "wb" )
				
				file_id = dict_data[ pic_attr ]
				file_id = ObjectId( file_id )
				if self.__file_store.exists( file_id ):
					# print type( file_id ), file_id
					file_obj_load = self.__file_store.get( file_id )
					file_obj.write( file_obj_load.read() )
					file_obj.close()
					pic_loaded = True
				else:
					self.__add_msg( Const[ "ERRMSG_IMG_ID_NOT_EXIST" ] % file_id )
			
			if not pic_loaded:
				fname = ""
			
			self.__set_image( pic_attr, fname, 0 )
			
		# 序號
		self.__show_serail( dict_data )
		
	# 利用索引值取得牌號ID
	def __get_key_by_index( self, index, type ):
		cache_index = self.__get_cache_data( type, "Index" )
		if index in cache_index:
			return cache_index[ index ]
		
		return INVALID_KEY
		
	# 利用介面選取取得牌號ID
	def __get_key_by_selection( self, type ):
		index = getattr( self.components, ATTR_MAP[ type ][ "ListCom" ] ).selection
		if index < 0:
			return index, INVALID_KEY
		
		return index, self.__get_key_by_index( index, type )
	
	# 數字判定
	def __str_digit_test( self, chk_text ):
		try:
			float( chk_text )
			return_val = True
		except:
			# 空字串幫你強轉 貼心ㄅ
			if chk_text == "":
				chk_text = "0"
				return_val = True
			else:
				return_val = False
		
		return return_val, chk_text
		
	# 由界面的文字輸入格 產生賣家資料
	def __gen_data_by_text_field( self ):
		# 作基本資料檢測
		com = self.components
		
		dict_data = {}
		dealer_map = ATTR_MAP[ "Dealer" ]
		
		attr_list = dealer_map[ "ALL_ATTR" ]
		ne_attr_list = dealer_map[ "NONEMPTY_ATTR" ]
		dig_attr_list = dealer_map[ "DIGIT_ATTR" ]
		
		for attr in attr_list:
			text_com = getattr( com, "TextField" + attr )
			chk_text = text_com.text
			attr_name = getattr( com, "StaticText" + attr ).text
			
			# 部分資料不可為空
			if attr in ne_attr_list and not len( chk_text ):
				self.__add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % attr_name )
				return None
			
			# 部分資料必須是數字
			if attr in dig_attr_list:
				return_val, chk_text = self.__str_digit_test( chk_text )
				if return_val:
					text_com.text = chk_text
				else:
					self.__add_msg( Const[ "TEXT_CHK_FAIL_MUST_DIGIT" ] % attr_name )
					return None
			
			dict_data[ attr ] = chk_text
		
		# 建立PK
		dict_data[ "_id" ] = dict_data[ "Name" ]
		return dict_data
		
	# 依買家資料產生界面顯示
	def __gen_list_ui_by_data( self ):
		com = self.components
		
		com.ListDealer.clear()
		
		cache_data = self.__get_cache_data( "Dealer", "Main" )
		cache_data.clear()
		cache_index = self.__get_cache_data( "Dealer", "Index" )
		cache_index.clear()
		
		index = -1
		for index, collection in enumerate( self.__dbtable_main.find().sort( DEALER_KEY, 1 ) ):
			dealer_name = collection[ DEALER_KEY ]
			
			cache_data[ dealer_name ] = index, collection
			cache_index[ index ] = dealer_name
			com.ListDealer.append( self.__gen_dealer_title( collection ) )
			
		return index
		
	# 產生賣家顯示在列表的字串
	def __gen_dealer_title( self, dict_data ):
		return dict_data[ DEALER_KEY ]
		
	# 產生道具顯示在列表的字串
	def __gen_item_title( self, dict_data ):
		return dict_data[ "ItemName" ]
		
	# 程序紀錄
	def __add_msg( self, msg, plus_pop = False ):
		if plus_pop:
			dialog.alertDialog( self, msg, Const[ "WARNING" ] )
		self.components.TextAreaLog.appendText( msg + "\n" )
		
	# 產生賣家拍品序號
	def __gen_item_serial( self ):
		# 建立PK 從表找出序號
		year_now = time.localtime( time.time() )[ 0 ] - 1911
		serial = 0
		for collection in self.__dbtable_item_serial.find():
			year = collection[ "year" ]
			serial = collection[ "serial" ]
		
		# 如果是空的 插入一筆資料 否則更新之
		table = self.__dbtable_item_serial
		if serial == 0:
			serial += 1
			table.insert( { "year": year_now, "serial": serial } )
		else:
			if year_now != year:
				serial = 0
			serial += 1
			table.update( { "year": year }, { "year": year_now, "serial": serial } )
		
		return "%d-%04d" % ( year_now, serial )
		
	# 由界面的文字輸入格 產生賣家拍品資料
	def __gen_item_data_by_text_field( self, dealer_name, item_key ):
		# 作基本資料檢測
		com = self.components
		
		# 沒有選擇賣家
		index, key = self.__get_key_by_selection( "Dealer" )
		if key not in self.__get_cache_data( "Dealer", "Main" ):
			self.__add_msg( Const[ "NO_DEALER_DATA" ] % key )
			return None
		
		dict_data = {}
		item_map = ATTR_MAP[ "Item" ]
		attr_list = item_map[ "ALL_ATTR" ]
		ne_attr_list = item_map[ "NONEMPTY_ATTR" ]
		dig_attr_list = item_map[ "DIGIT_ATTR" ]
		
		for attr in attr_list:
			text_com = getattr( com, "TextField" + attr )
			chk_text = text_com.text
			attr_name = getattr( com, "StaticText" + attr ).text
			
			# 部分資料不可為空
			if attr in ne_attr_list and not len( chk_text ):
				self.__add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % attr_name )
				return None
			
			# 部分資料必須是數字
			if attr in dig_attr_list:
				return_val, chk_text = self.__str_digit_test( chk_text )
				if return_val:
					text_com.text = chk_text
				else:
					self.__add_msg( Const[ "TEXT_CHK_FAIL_MUST_DIGIT" ] % attr_name )
					return None
			
			dict_data[ attr ] = chk_text
		
		# 來源賣家
		dict_data[ "SrcDealer" ] = key
		
		# 圖示路徑
		for attr, com_name in ITEM_COM_DATA.iteritems():
			canvas = getattr( com, com_name )
			
			if hasattr( canvas, "path" ):
				try:
					file_obj = open( canvas.path, "rb" )
					file_id = self.__file_store.put( file_obj )
					file_id = str( file_id )
					# print file_id
				except:
					file_id = None
			else:
				file_id = None
			
			dict_data[ attr ] = file_id
		
		# 如果有填拍品編號
		lot_number = dict_data[ "LotNO" ]
		if lot_number != "":
			# 檢查拍品編號是否存在
			query_result = self.__dbtable_auction.find( { "AuctionId": lot_number } ).count()
			if not query_result:
				self.__add_msg( Const[ "TEXT_LOT_NO_NOT_EXIST" ] % lot_number )
				return None
				
			# 檢查拍品編號唯一性 有改or新增才判斷
			if not item_key or lot_number != self.__dbtable_item.find( { "_id": item_key } )[ 0 ][ "LotNO" ]:
				query_result = self.__dbtable_item.find( { "LotNO": lot_number } ).count()
				if query_result:
					self.__add_msg( Const[ "TEXT_LOT_NO_REPEATED" ] % lot_number )
					return None

		return dict_data
		
	# 嘗試新增道具
	def __add_item_dict( self, dict_data ):
		try:
			self.__dbtable_item.insert( dict_data )
		except:
			self.__add_msg( Const[ "ERRMSG_NAME_REPEAT" ], True )
			return 0
			
		return 1
	
	# 依道具資料產生界面顯示
	def __gen_item_list_ui_by_data( self, dealer_name ):
		com = self.components
		
		com.ListItem.clear()
		
		cache_data = self.__get_cache_data( "Item", "Main" )
		cache_data.clear()
		cache_index = self.__get_cache_data( "Item", "Index" )
		cache_index.clear()
		
		index = -1
		for index, collection in enumerate( self.__dbtable_item.find( { "SrcDealer": dealer_name } ).sort( ITEM_KEY, 1 ) ):
			item_key = collection[ ITEM_KEY ]
			
			cache_data[ item_key ] = index, collection
			cache_index[ index ] = item_key
			
			com.ListItem.append( self.__gen_item_title( collection ) )
			
		return index
		
	# 設定圖片顯示
	def __set_image( self, attr_name, path, need_msg ):
		com_name = ITEM_COM_DATA[ attr_name ]
		com = self.components
		canvas = getattr( com, com_name )
		
		canvas.visible = False
		
		try:
			canvas.visible = True
			canvas.clear()
			canvas.autoRefresh = 1
			
			bmp_obj = graphic.Bitmap( path )
			canvas.drawBitmapScaled( bmp_obj, ( 0, 0 ), canvas.size )
			
			canvas.path = path
		
		except:
			if need_msg:
				self.__add_msg( Const[ "ERRMSG_INVALID_IMG_FILE" ], 1 )
	
	# 按下 設定圖片 按鈕
	def __set_image_path( self, attr_name ):
		result = dialog.openFileDialog( title = Const[ "OPEN_FILE_TITLE" ] )
		if not result.paths:
			return
		
		path = result.paths[ 0 ]
		self.__set_image( attr_name, path, 1 )
	
	def on_ButtonSetPackImg_mouseClick( self, event ):
		self.__set_image_path( "PackImg" )
	
	def on_ButtonSetBuildImg_mouseClick( self, event ):
		self.__set_image_path( "BuildImg" )
		
	# 右鍵移除圖片
	def on_ButtonSetPackImg_mouseContextDown( self, event ):
		self.__on_del_image( "PackImg" )
		
	def on_ButtonSetBuildImg_mouseContextDown( self, event ):
		self.__on_del_image( "BuildImg" )

	def __on_del_image( self, attr_name ):
		result = dialog.singleChoiceDialog( self, "", Const[ "STATICTEXTDELIMAGETITLE" ], [ Const[ "STATICTEXTDEL" ], Const[ "STATICTEXTKEEP" ] ] )
		if result.selection == Const[ "STATICTEXTDEL" ]:
			self.__set_image( attr_name, "", 0 )
			
if __name__ == '__main__':
	app = model.Application(MyBackground)
	app.MainLoop()
