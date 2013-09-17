#-*- coding: cp950 -*-

"""
__version__ = "$Revision: 1.3 $"
__date__ = "$Date: 2004/04/14 02:38:47 $"
"""

import sys
sys.path.append( "..\\" )

from PythonCard import dialog, model
from threading import Thread
from socket import *
from Modules.bidder_data_handler import bidder_data_handler as bdh
from Modules.bidder_data_handler_const import *
from BidderDataServer.server_id import CONNECTER_DATA_INPUT
import re

# 連線port
DEFAULT_PORT	= 5566

# 牌號最大位數
MAX_DIGIT			= 5

# 暫存的IP存檔路徑
CACHED_IP_PATH	= "cached_ip.ini"

# 所有常數字串
Const			= {	"STATICTEXTNAME":						"準買家姓名",
							"STATICTEXTCOMPANY":				"公司名稱",
							"STATICTEXTCAREERTITLE":		"職稱",
							"STATICTEXTIDNUMBER":				"身分證/護照號碼",
							"STATICTEXTTEL":						"電話",
							"STATICTEXTFAX":						"傳真",
							"STATICTEXTADDRESS":				"地址",
							"STATICTEXTEMAIL":					"E-Mail",
							"STATICTEXTBANK":						"往來銀行/分行",
							"STATICTEXTBANKACC":				"帳號",
							"STATICTEXTBANKCONTACT":		"銀行連絡人",
							"STATICTEXTBANKCONTACTTEL":	"連絡電話",
							"STATICTEXTCREDITCARDID":		"信用卡號碼",
							"STATICTEXTCREDITCARDTYPE":	"信用卡類別",
							"STATICTEXTBIDDERID":				"牌號",
							"BUTTONTEXTNEWFILE":				"新建檔案",
							"BUTTONTEXTOPEN":						"匯入檔案",
							"BUTTONTEXTCONNECT":				"建立連線",
							"BUTTONTEXTSENDSINGLE":			"單筆送出",
							"BUTTONTEXTSENDALL":				"全部送出",
							"BUTTONTEXTADDBIDDER":			"新增買家",
							"BUTTONTEXTFIXBIDDER":			"修正資料",
							"BUTTONTEXTDELBIDDER":			"刪除買家",
							"NEW_FILE_TITLE":						"請建立一個資料檔",
							"FILE_RULE":								"買家資料檔(*.txt)|*.txt",
							"NEW_FILE_FAILED":					"你取消了資料檔的建立",
							"NEW_FILE_SUCCESS":					u"成功建立了新的資料檔，路徑為%s",
							"FILE_NOT_IMPORT":					"<<你還沒有匯入你的資料檔>>",
							"OPEN_FILE_TITLE":					"請開啟一個資料檔",
							"OPEN_FILE_FAILED":					"你取消了資料檔的開啟",
							"OPEN_FILE_SUCCESS":				u"成功匯入資料檔，路徑為%s",
							"OPEN_FILE_NAME":						u"資料檔<%s>",
							"TEXT_CHK_FAIL_EMPTY":			u"[%s]不可為空",
							"BIDDER_ID_LEN_EXCEEDED":		"牌號位數過大，請修正",
							"BIDDER_ID_HAS_FOUR":				"牌號不可含有""4""或是""7""",
							"BIDDER_ID_NOT_DIGIT":			"牌號必須是數字，請修正",
							"WARNING":									"警告",
							"BIDDER_ID_REPEAT":					u"牌號已和[%s]重複",
							"ADD_BIDDER_SUCCESS":				u"成功新增了買家[%s]",
							"NO_SELECTED_BIDDER":				"請選擇一個買家",
							"FIX_BIDDER_DONE":					u"已成功修改[%s]的資料",
							"NO_BIDDER_DATA":						"查無買家資料!! ID = %d",
							"DEL_BIDDER_SUCCESS":				u"成功刪除了買家[%s]",
							"WIN_TITLE_TEXT":						"台灣世家拍賣-競拍者資料建檔系統",
							"SAVE_FILE_SUCCESS":				"資料檔已儲存",
							"MAX_DIGIT_NOW":						"目前牌號最大位數為%d位",
							"UNSAVED_MODS_TITLE":				"未儲存資料的處理方式",
							"UNSAVED_MODS_DENY":				"你目前還有修改中的未儲存資料，無法進行此操作",
							"UNSAVED_MODS_QUESTION":		"你目前還有修改中的未儲存資料，請問你接下來要....",
							"UNSAVED_MODS_QUIT":				u"不修改了，直接關閉",
							"UNSAVED_MODS_RESUME":			"繼續編輯資料",
							"PLZ_ENTER_AUCTION_IP":			"請輸入拍賣中心的IP位址",
							"READY_TO_CONNECT":					"準備開始連線",
							"DEFAULT_IP":								"127.0.0.1",
							"CONNECT_FAILED":						"無法連線到[%s]",
							"CONNECT_SUCCESS":					"成功連線到[%s]",
							"BIDDER_DATA_SENT":					u"已送出買家[%s]的完整資料至拍賣中心",
							"CONNECT_BEGIN":						u"正在連線至[%s]，請耐心等候",
						}

# 所有開檔後才能使用的元件屬性
ACTIVATE_ATTRS = []
for attr in BIDDER_ATTRS:
	ACTIVATE_ATTRS.append( "TextField" + attr )
ACTIVATE_ATTRS += [ "ButtonConnect", "ButtonAddBidder", "ButtonFixBidder", "ButtonDelBidder" ]

# 所有連線後才能使用的元件屬性
CONNECT_ATTRS = [ "ButtonSendSingle", "ButtonSendAll" ]

# 收訊息執行緒物件
class class_socket_recv( Thread ):
	def __init__( self, win_ob, socket_ob ):
		Thread.__init__( self )
		
		self.win_ob			= win_ob
		self.socket_ob	= socket_ob
		
		self.last_remain_str = ""
		
	def do_recv_data_input( self, recv_str ):
		print "==="
		print "do_recv_data_input"
		print "len_ori = %d" % len( recv_str )
		# 要先接在一起
		recv_str = self.last_remain_str + recv_str
		print "len_now = %d" % len( recv_str )
		# 用封包分隔字串切開封包
		recv_str_list = recv_str.split( SEP_PACKET )
		# cache最後面的剩餘字串(一般來說沒超過buffer都會是"")
		self.last_remain_str = recv_str_list[ -1 ]
		
		if self.last_remain_str != "":
			print "last_remain_str set to %s" % self.last_remain_str
			
		else:
			print "perfect packet"
		
		recv_str_list.pop( -1 )
		for index, recv_str_iter in enumerate( recv_str_list ):
			print "-packet NO.%d, str = %s" % ( index, recv_str_iter )
			self.win_ob.add_msg( recv_str_iter )
			
	def run( self ):
		while True:
			recv_str = self.socket_ob.recv( 1024 )
			self.do_recv_data_input( recv_str )
		
# 連線執行緒物件
class class_socket_connect( Thread ):
	def __init__( self, win_ob ):
		Thread.__init__( self )
		
		self.win_ob			= win_ob;
		self.socket_ob	= socket( AF_INET, SOCK_STREAM )
		
	def run( self ):
		ip, port = self.win_ob.connect_ip.encode( STR_CODE ), DEFAULT_PORT
		try:
			self.socket_ob.connect( ( ip, port ) )
		except:
			self.win_ob.add_msg( Const[ "CONNECT_FAILED" ] % ip )
			return
			
		self.win_ob.add_msg( Const[ "CONNECT_SUCCESS" ] % ip )
		self.win_ob.connected_socket_ob = self.socket_ob
		
		com = self.win_ob.components
		for attr in CONNECT_ATTRS:
			getattr( com, attr ).enabled = True
			
		self.revc_process = class_socket_recv( self.win_ob, self.socket_ob )
		self.revc_process.start()
		
		# 先送驗證資訊
		self.socket_ob.send( CONNECTER_DATA_INPUT )

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
		
		# 按鈕
		com.ButtonNewFile.label						= Const[ "BUTTONTEXTNEWFILE" ]
		com.ButtonOpen.label							= Const[ "BUTTONTEXTOPEN" ]
		com.ButtonConnect.label						= Const[ "BUTTONTEXTCONNECT" ]
		com.ButtonSendSingle.label				= Const[ "BUTTONTEXTSENDSINGLE" ]
		com.ButtonSendAll.label						= Const[ "BUTTONTEXTSENDALL" ]
		com.ButtonAddBidder.label					= Const[ "BUTTONTEXTADDBIDDER" ]
		com.ButtonFixBidder.label					= Const[ "BUTTONTEXTFIXBIDDER" ]
		com.ButtonDelBidder.label					= Const[ "BUTTONTEXTDELBIDDER" ]
		
		# 顯示檔名
		com.StaticTextDataFileName.text		= Const[ "FILE_NOT_IMPORT" ]
		
		# 建立買家資料結構
		self.bidder_data_map = {}
		
		# 目前牌號最大位數
		self.max_digit = 0
		
		# 目前的檔案路徑
		self.cur_file_path = ""
		
		# 目前的資料是否已儲存
		self.saved_flag = True
		
		# 網路相關
		# 收封包的執行緒
		self.socket_process = None
		self.connect_ip = u""
		self.connected_socket_ob = None
		
	# 讀取暫存的IP
	def load_cached_ip( self ):
		try:
			file_ob = open( CACHED_IP_PATH, "r" )
			
		except:
			return Const[ "DEFAULT_IP" ]
		
		line = file_ob.readline()
		if line[ -1 ] == "\n":
			line = line[ : -1 ]
		
		file_ob.close()
		return line
		
	# 儲存暫存的IP
	def save_cached_ip( self ):
		try:
			file_ob = open( CACHED_IP_PATH, "w" )
			
		except:
			print "open file %s failed" % CACHED_IP_PATH
			return
		
		file_ob.write( self.connect_ip )
		file_ob.close()
		
	# 按下 建立連線 按鈕
	def on_ButtonConnect_mouseClick( self, event ):
		result = dialog.textEntryDialog( self, Const[ "PLZ_ENTER_AUCTION_IP" ], Const[ "READY_TO_CONNECT" ], self.load_cached_ip() )
		if not result.accepted:
			return
		
		self.connect_ip = result.text
		self.save_cached_ip()
		
		if self.socket_process:
			del self.socket_process
		self.socket_process = class_socket_connect( self )
		self.socket_process.start()
		
		self.add_msg( Const[ "CONNECT_BEGIN" ] % self.connect_ip )
		
	# 按下 單筆資料送出 按鈕
	def on_ButtonSendSingle_mouseClick( self, event ):
		if not self.saved_flag:
			self.add_msg( Const[ "UNSAVED_MODS_DENY" ] )
			return
		
		self.send_bidder_data( self.get_bidder_id_by_selection() )
		
	# 按下 所有資料送出 按鈕
	def on_ButtonSendAll_mouseClick( self, event ):
		if not self.saved_flag:
			self.add_msg( Const[ "UNSAVED_MODS_DENY" ] )
			return
		
		for bidder_id in self.bidder_data_map:
			self.send_bidder_data( bidder_id )
		
	# 送出資料
	def send_bidder_data( self, bidder_id ):
		if bidder_id not in self.bidder_data_map:
			self.add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return
		
		bidder_data = self.bidder_data_map[ bidder_id ]
		self.connected_socket_ob.send( bdh.gen_bidder_str_data( bidder_data ) + SEP_PACKET )
		
		self.add_msg( Const[ "BIDDER_DATA_SENT" ] % self.gen_bid_id_plus_name_str( bidder_id ) )
	
	# 按下 新建檔案 按鈕
	def on_ButtonNewFile_mouseClick( self, event ):
		result = dialog.saveFileDialog( title = Const[ "NEW_FILE_TITLE" ], wildcard = Const[ "FILE_RULE" ] )
		if not result.paths:
			self.add_msg( Const[ "NEW_FILE_FAILED" ] )
			return
		
		# 建立一個空白文字檔
		path = result.paths[ 0 ]
		file_ob = open( path, "w" )
		file_ob.close()
		self.add_msg( Const[ "NEW_FILE_SUCCESS" ] % path )
		
		self.run_open_file( path )
	
	# 按下 匯入檔案 按鈕
	def on_ButtonOpen_mouseClick( self, event ):
		result = dialog.openFileDialog( title = Const[ "OPEN_FILE_TITLE" ], wildcard = Const[ "FILE_RULE" ] )
		if not result.paths:
			self.add_msg( Const[ "OPEN_FILE_FAILED" ] )
			return
		
		self.run_open_file( result.paths[ 0 ] )
	
	# 執行匯入檔案
	def run_open_file( self, path ):
		# 讀取既有的檔案
		self.cur_file_path = path
		
		# 重置買家資料結構
		self.bidder_data_map, max_digit = bdh.load_data( path )
		
		self.gen_list_ui_by_bidder_data()
		
		# 設定最大位數
		self.set_max_digit( max_digit )
		
		# 更新列表的標題
		fname = path[ path.rfind( "\\" ) + 1 : ]
		com = self.components
		com.StaticTextDataFileName.text = Const[ "OPEN_FILE_NAME" ] % fname
		
		# 啟用各個元件
		for attr in ACTIVATE_ATTRS:
			ui_unit = getattr( com, attr )
			ui_unit.enabled = True
			if attr.find( "TextField" ) > -1:
				ui_unit.text = ""
		
		self.update_list_ui()
		self.add_msg( Const[ "OPEN_FILE_SUCCESS" ] % path )
		self.saved_flag = True
	
	# 按下 新增使用者 按鈕
	def on_ButtonAddBidder_mouseClick( self, event ):
		# 如果無法取得資料 下面的function會送系統訊息
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return
		
		index = self.add_bidder_data( bidder_data )
		if index < 0:
			return
		
		self.components.ListBidders.selection = index
		self.add_msg( Const[ "ADD_BIDDER_SUCCESS" ] % self.gen_bid_id_plus_name_str( int( bidder_data[ "BidderID" ] ) ) )
		self.save_bidder_data()
	
	# 按下 編輯買家資料 按鈕
	def on_ButtonFixBidder_mouseClick( self, event ):
		com = self.components
		
		# 沒有選取的項目
		selected_str = com.ListBidders.stringSelection
		if selected_str == "":
			self.add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
			
		# 如果無法取得資料 下面的function會送系統訊息
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return
		
		# 取得當前所選擇的ID
		bidder_id_str	= bidder_data[ "BidderID" ]
		bidder_id			= int( bidder_id_str )
		bidder_id_now	= self.get_bidder_id_by_selection()
		
		# 如果沒改牌號 那就是單純改資料
		index = com.ListBidders.selection
		if bidder_id == bidder_id_now:
			self.bidder_data_map[ bidder_id ] = bidder_data
			new_title = self.gen_bid_id_plus_name_str( bidder_id )
			# 以防萬一 List的文字也更新一下
			com.ListBidders.setString( index, new_title )
			
			self.add_msg( Const[ "FIX_BIDDER_DONE" ] % new_title )
			self.save_bidder_data()
			return
		
		# 如果改了牌號 不可以跟現有的重複
		if bidder_id in self.bidder_data_map:
			self.add_msg( Const[ "BIDDER_ID_REPEAT" ] % self.gen_bid_id_plus_name_str( bidder_id ), True )
			return
			
		# 先刪掉現在的
		self.del_bidder_by_index( index )
		
		index = self.add_bidder_data( bidder_data )
		if index < 0:
			return
		
		self.components.ListBidders.selection = index
		self.add_msg( Const[ "FIX_BIDDER_DONE" ] % self.gen_bid_id_plus_name_str( bidder_id ) )
		self.save_bidder_data()
		
	# 按下買家列表資料
	def on_ListBidders_select( self, event = None ):
		bidder_id = self.get_bidder_id_by_selection()
		if bidder_id not in self.bidder_data_map:
			self.add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return
		
		bidder_data = self.bidder_data_map[ bidder_id ]
		for attr in BIDDER_ATTRS:
			getattr( self.components, "TextField" + attr ).text = bidder_data[ attr ]
			
		self.saved_flag = True
		
	# 按下 刪除買家資料 按鈕
	def on_ButtonDelBidder_mouseClick( self, event ):
		com = self.components
		
		# 沒有選取的項目
		index = com.ListBidders.selection
		if index < 0:
			self.add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		bidder_id = self.get_bidder_id_by_selection()
		bid_id_plus_name_str = self.gen_bid_id_plus_name_str( bidder_id )
		
		result = self.del_bidder_by_index( index )
		if not result:
			return
		
		self.update_list_ui()
			
		self.add_msg( Const[ "DEL_BIDDER_SUCCESS" ] % bid_id_plus_name_str )
		self.save_bidder_data()
		
	# 按下 [X] 按鈕
	def on_close( self, event ):
		# 已經儲存了 直接關閉
		if self.saved_flag:
			event.skip()
			return
		
		# 仍然選擇關閉 才真的關閉
		result = dialog.singleChoiceDialog( self, Const[ "UNSAVED_MODS_QUESTION" ], Const[ "UNSAVED_MODS_TITLE" ], [ Const[ "UNSAVED_MODS_RESUME" ], Const[ "UNSAVED_MODS_QUIT" ] ] )
		if result.selection == Const[ "UNSAVED_MODS_QUIT" ]:
			event.skip()
			return
	
	# 儲存資料
	def save_bidder_data( self ):
		bdh.save_data( self.cur_file_path, self.bidder_data_map )
		
		self.add_msg( Const[ "SAVE_FILE_SUCCESS" ] )
		self.saved_flag = True
	
	# 更新列表界面
	def update_list_ui( self ):
		# 如果還有剩下的資料 選擇第一個 否則清空所有文字
		com = self.components
		if com.ListBidders.getCount() > 0:
			com.ListBidders.selection = 0;
			self.on_ListBidders_select()
			
		else:
			for attr in BIDDER_ATTRS:
				getattr( com, "TextField" + attr ).text = ""
		
	# 由索引值刪除的買家資料
	def del_bidder_by_index( self, index ):
		bidder_id = self.get_bidder_id_by_index( index )
		if bidder_id not in self.bidder_data_map:
			self.add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return False
			
		self.bidder_data_map.pop( bidder_id )
		self.components.ListBidders.delete( index )
		return True
		
	# 利用索引值取得牌號ID
	def get_bidder_id_by_index( self, index ):
		ui_str = self.components.ListBidders.getString( index )
		if ui_str == "":
			return -1;
		
		return int( ui_str[ : ui_str.find( ":" ) ] )
		
	# 利用介面選取取得牌號ID
	def get_bidder_id_by_selection( self ):
		index = self.components.ListBidders.selection
		if index < 0:
			return -1;
		
		return self.get_bidder_id_by_index( index )
	
	# 新增買家資料
	def add_bidder_data( self, bidder_data ):
		result, bidder_id, is_update_len, new_max_digit = bdh.add_data( bidder_data, self.max_digit, self.bidder_data_map )
		
		# 牌號不可重複
		if not result:
			self.add_msg( Const[ "BIDDER_ID_REPEAT" ] % self.gen_bid_id_plus_name_str( bidder_id ), True )
			return -1
			
		# 插入在正確的UI位置
		bidder_id_list = self.bidder_data_map.keys()
		bidder_id_list.sort()
		index = bidder_id_list.index( bidder_id );
		bid_id_plus_name_str = self.gen_bid_id_plus_name_str( bidder_id )
		
		self.components.ListBidders.insertItems( [ bid_id_plus_name_str ], index )
		
		# 最大牌號被改了 刷新整個列表
		if is_update_len:
			# 介面端
			self.gen_list_ui_by_bidder_data()
			
			self.set_max_digit( new_max_digit )
		return index
	
	# 設定當前牌號最大位數
	def set_max_digit( self, max_digit ):
		self.max_digit = max_digit
		self.add_msg( Const[ "MAX_DIGIT_NOW" ] % max_digit )
	
	# 由界面的文字輸入格 產生買家資料
	def gen_bidder_data_by_text_field( self ):
		# 作基本資料檢測
		com = self.components
		
		# 所有資料皆不可為空
		bidder_data = {}
		for attr in BIDDER_ATTRS:
			chk_text = getattr( com, "TextField" + attr ).text
			if attr in NONEMPTY_ATTRS and not len( chk_text ):
				self.add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % getattr( com, "StaticText" + attr ).text )
				return None
			
			bidder_data[ attr ] = chk_text
		
		# 牌號位數不可超過最大值
		bidder_id_str = bidder_data[ "BidderID" ]
		bidder_id_len = len( bidder_id_str )
		if bidder_id_len > MAX_DIGIT:
			self.add_msg( Const[ "BIDDER_ID_LEN_EXCEEDED" ] )
			return None
			
		# 牌號過濾規則(不可有4, 7)
		re_ob = re.compile( "[47]+" )
		if re_ob.search( bidder_id_str ):
			self.add_msg( Const[ "BIDDER_ID_HAS_FOUR" ] )
			return None
			
		# 牌號必須是純數字
		if not bidder_id_str.isdigit():
			self.add_msg( Const[ "BIDDER_ID_NOT_DIGIT" ] )
			return None
		
		# 修正牌號字串
		bidder_data[ "BidderID" ] = bdh.complete_str_by_zero( bidder_data[ "BidderID" ], self.max_digit )
		return bidder_data
		
	# 依買家資料產生界面顯示
	def gen_list_ui_by_bidder_data( self ):
		com = self.components
		
		com.ListBidders.clear()
		bidder_data_ui = []
		# 先排序
		bidder_id_list = self.bidder_data_map.keys()
		bidder_id_list.sort()
		for bidder_id in bidder_id_list:
			bidder_data_ui.append( self.gen_bid_id_plus_name_str( bidder_id ) )
		
		com.ListBidders.insertItems( bidder_data_ui, 0 )
		
	# 產生牌號:姓名的字串
	def gen_bid_id_plus_name_str( self, bidder_id ):
		bidder_data = self.bidder_data_map[ bidder_id ]
		return "%s: %s" % ( bidder_data[ "BidderID" ], bidder_data[ "Name" ] )
		
	# 程序紀錄
	def add_msg( self, msg, plus_pop = False ):
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
