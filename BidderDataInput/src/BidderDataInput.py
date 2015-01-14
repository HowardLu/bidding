#-*- coding: cp950 -*-

"""
__version__ = "$Revision: 1.3 $"
__date__ = "$Date: 2004/04/14 02:38:47 $"
"""

from PythonCard import dialog, model
import pymongo, sys, re
import xlwt

# �R�a�Ҧ��ݩ�
BIDDER_ATTRS = [	"BidderID", "Name", "Company", "CareerTitle", "IDNumber", "Tel", "Fax", "Address", "EMail", "Bank", "BankAcc",
									"BankContact", "BankContactTel", "CreditCardID", "CreditCardType", "Auctioneer", "GuaranteeCost"
								]

BIDDER_NONEMPTY_ATTRS = [ "BidderID", "Name", "IDNumber", "Tel", "Auctioneer" ]

# �R�a��ƿ�X���|
EXPORT_PATH	= "bidder_export.xls"

# �s�uport
DB_CFG_PORT	= 27017

# �w�]��I�覡����
GUARANTEE_INIT	= 0

# �g����login account
DB_LOGIN_ACC = { "showmethemoney": None, "superjunior": "S", "igsigs": "N" }

# �L�ĵP��
INVALID_ID	= -1

NONEMPTY_ATTRS = BIDDER_NONEMPTY_ATTRS

# �Ȧs��IP�s�ɸ��|
CACHED_IP_PATH	= "cached_ip.ini"

# �Ҧ��`�Ʀr��
Const			= {	"STATICTEXTNAME":						"�ǶR�a�m�W",
							"STATICTEXTCOMPANY":				"���q�W��",
							"STATICTEXTCAREERTITLE":		"¾��",
							"STATICTEXTIDNUMBER":				"������/�@�Ӹ��X",
							"STATICTEXTTEL":						"�q��",
							"STATICTEXTFAX":						"�ǯu",
							"STATICTEXTADDRESS":				"�a�}",
							"STATICTEXTEMAIL":					"E-Mail",
							"STATICTEXTBANK":						"���ӻȦ�/����",
							"STATICTEXTBANKACC":				"�b��",
							"STATICTEXTAUCTIONEER":			"����",
							"STATICTEXTBANKCONTACT":		"�Ȧ�s���H",
							"STATICTEXTBANKCONTACTTEL":	"�s���q��",
							"STATICTEXTCREDITCARDID":		"�H�Υd���X",
							"STATICTEXTCREDITCARDTYPE":	"�H�Υd���O",
							"STATICTEXTBIDDERID":				"�P��",
							"STATICTEXTGUARANTEETYPE":	"�O�Ҫ�ú��",
							"STATICTEXTGUARANTEECOST":	"���B",
							"BUTTONTEXTNEWFILE":				"�s���ɮ�",
							"BUTTONTEXTOPEN":						"�פJ�ɮ�",
							"BUTTONTEXTCONNECT":				"�إ߳s�u",
							"BUTTONTEXTSENDSINGLE":			"�浧�e�X",
							"BUTTONTEXTSENDALL":				"�����e�X",
							"BUTTONTEXTADDBIDDER":			"�s�W�R�a",
							"BUTTONTEXTFIXBIDDER":			"�ץ����",
							"BUTTONTEXTDELBIDDER":			"�R���R�a",
							"BUTTONTEXTCLEAR":					"�M�����",
							"BUTTONTEXTSEARCH":					"�j�M/��s",
							"BUTTONTEXTEXPORT":					"�ץX",
							"COMBOBOXGUARANTEETYPE":		u"��I�覡",
							"COMBOBOXGUARANTEE_LIST":		[ u"�x���{�r", u"�H�����{�r", u"�����{�r", u"�H�Υd", u"���p�d", u"VIP" ],
							"NEW_FILE_TITLE":						"�Ыإߤ@�Ӹ����",
							"FILE_RULE":								"�R�a�����(*.txt)|*.txt",
							"NEW_FILE_FAILED":					"�A�����F����ɪ��إ�",
							"NEW_FILE_SUCCESS":					u"���\�إߤF�s������ɡA���|��%s",
							"FILE_NOT_IMPORT":					"<<�A�|���P��Ʈw�إ߳s�u>>",
							"OPEN_FILE_TITLE":					"�ж}�Ҥ@�Ӹ����",
							"OPEN_FILE_FAILED":					"�A�����F����ɪ��}��",
							"OPEN_FILE_SUCCESS":				u"���\�פJ����ɡA���|��%s",
							"OPEN_FILE_NAME":						u"�����<%s>",
							"TEXT_CHK_FAIL_EMPTY":			u"[%s]���i����",
							"BIDDER_ID_LEN_EXCEEDED":		"�P����ƹL�j�A�Эץ�",
							"BIDDER_ID_HAS_FOUR":				"�P�����i�t��""4""�άO""7""",
							"BIDDER_ID_NOT_DIGIT":			"�P�������O�Ʀr�A�Эץ�",
							"GUARANTEECOST_NOT_DIGIT":	"�O��ú�Ǫ��B�����O�Ʀr�A�Эץ�",
							"WARNING":									"ĵ�i",
							"BIDDER_ID_REPEAT":					u"�P���w�M[%s]����",
							"ADD_BIDDER_SUCCESS":				u"���\�s�W�F�R�a[%s]",
							"NO_SELECTED_BIDDER":				"�п�ܤ@�ӶR�a",
							"FIX_BIDDER_DONE":					u"�w���\�ק�[%s]�����",
							"NO_BIDDER_DATA":						"�d�L�R�a���!! ID = %d",
							"DEL_BIDDER_SUCCESS":				u"���\�R���F�R�a[%s]",
							"WIN_TITLE_TEXT":						"�v��̸�ƫ��ɨt��",
							"SAVE_FILE_SUCCESS":				"����ɤw�x�s",
							"MAX_DIGIT_NOW":						"�ثe�P���̤j��Ƭ�%d��",
							"UNSAVED_MODS_TITLE":				"���x�s��ƪ��B�z�覡",
							"UNSAVED_MODS_DENY":				"�A�ثe�٦��ק襤�����x�s��ơA�L�k�i�榹�ާ@",
							"UNSAVED_MODS_QUESTION":		"�A�ثe�٦��ק襤�����x�s��ơA�аݧA���U�ӭn....",
							"UNSAVED_MODS_QUIT":				u"���ק�F�A��������",
							"UNSAVED_MODS_RESUME":			"�~��s����",
							"PLZ_ENTER_AUCTION_IP":			"�п�J��Ʈw��IP��}",
							"PLZ_ENTER_LOGIN_CODE":			"�п�J�n�J�K�X",
							"READY_TO_CONNECT":					"�ǳƶ}�l�s�u",
							"READY_TO_LOGIN":						"�n�J��",
							"DEFAULT_IP":								"127.0.0.1",
							"CONNECT_FAILED":						"�L�k�s�u��[%s]",
							"CONNECT_SUCCESS":					"���\�s�u��[%s]",
							"BIDDER_DATA_SENT":					u"�w�e�X�R�a[%s]�������Ʀܩ�椤��",
							"CONNECT_BEGIN":						u"���b�s�u��[%s]�A�Э@�ߵ���",
							"ERRMSG_DB_CNCT_FAIL":			u"�L�k�s�u���Ʈw( %s, %d )",
							"ERRMSG_DB_CNCT_OK":				u"���\�s�u���Ʈw( %s, %d )",
							"ERRMSG_EXPORT_OK":					u"�w�ץX�R�a��Ʀ�""%s""",
						}

# ������檫��
class MyBackground( model.Background ):
	# �����l��
	def on_initialize( self, event ):
		# ��l�ƩҦ���r ���D
		self.title = Const[ "WIN_TITLE_TEXT" ]
		
		# �R�A��r
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
		
		# ���s
		com.ButtonAddBidder.label					= Const[ "BUTTONTEXTADDBIDDER" ]
		com.ButtonFixBidder.label					= Const[ "BUTTONTEXTFIXBIDDER" ]
		com.ButtonDelBidder.label					= Const[ "BUTTONTEXTDELBIDDER" ]
		com.ButtonClear.label							= Const[ "BUTTONTEXTCLEAR" ]
		com.ButtonSearch.label						= Const[ "BUTTONTEXTSEARCH" ]
		com.ButtonExport.label						= Const[ "BUTTONTEXTEXPORT" ]
		
		# ���
		com.ComboBoxGuaranteeType.text		= Const[ "COMBOBOXGUARANTEE_LIST" ][ GUARANTEE_INIT ]
		for type in Const[ "COMBOBOXGUARANTEE_LIST" ]:
			com.ComboBoxGuaranteeType.append( type )
		com.ComboBoxGuaranteeType.SetEditable( False )
		
		# ���յn�J
		while True:
			login_result, self.__login_key = self.__login_process()
			if login_result:
				break
		
		if self.__login_key:
			com.TextFieldAuctioneer.text = self.__login_key
			com.TextFieldAuctioneer.SetEditable( False )
			com.TextFieldAuctioneer.enabled = False
			
		# ���ճs�W��Ʈw
		while True:
			connect_ip = self.__connection_process()
			if connect_ip:
				break
			
		self.__add_msg( Const[ "ERRMSG_DB_CNCT_OK" ] % ( connect_ip, DB_CFG_PORT ), True )
		
		# ���o��Ʈw������ƪ�
		self.__mongo_db = self.__mongo_client.bidding_data
		self.__dbtable_buyer = self.__mongo_db.buyer_table
		
		# ������ܩҦ��R�a
		self.__gen_list_ui_by_bidder_data()
		
	# Ū���Ȧs��IP
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
		
	# �x�s�Ȧs��IP
	def __save_cached_ip( self, connect_ip ):
		try:
			file_ob = open( CACHED_IP_PATH, "w" )
			
		except:
			return
		
		file_ob.write( connect_ip )
		file_ob.close()
		
	# ���յn�J
	def __login_process( self ):
		result = dialog.textEntryDialog( self, Const[ "PLZ_ENTER_LOGIN_CODE" ], Const[ "READY_TO_LOGIN" ] )
		if not result.accepted:
			return False
		
		code = result.text
		
		if code not in DB_LOGIN_ACC:
			return False, False
		
		return True, DB_LOGIN_ACC[ code ]
		
	# ���ճs�W��Ʈw
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
		
	# ���շs�W�ϥΪ�
	def __add_buyer( self, bidder_data ):
		try:
			self.__dbtable_buyer.insert( bidder_data )
		except:
			self.__add_msg( Const[ "BIDDER_ID_REPEAT" ] % self.__gen_bid_id_plus_name_str( bidder_data[ "BidderID_int" ] ), True )
			return 0
			
		return 1
		
	# ���U �s�W�ϥΪ� ���s
	def on_ButtonAddBidder_mouseClick( self, event ):
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return

		# ���J��ƨí��s��z����
		if not self.__add_buyer( bidder_data ):
			return
		
		self.__gen_list_ui_by_bidder_data()
		bidder_id = bidder_data[ "BidderID_int" ]
		self.__add_msg( Const[ "ADD_BIDDER_SUCCESS" ] % self.__gen_bid_id_plus_name_str( bidder_id ) )
		
		index, bidder_data = self.__cached_data[ bidder_id ]
		self.components.ListBidders.selection = index
		
	# ���U �s��R�a��� ���s
	def on_ButtonFixBidder_mouseClick( self, event ):
		# �S�����������
		index, bidder_id_ori = self.get_bidder_id_by_selection()
		if bidder_id_ori not in self.__cached_data:
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
			
		# �p�G�L�k���o��� �U����function�|�e�t�ΰT��
		bidder_data = self.gen_bidder_data_by_text_field()
		if not bidder_data:
			return
		
		bidder_id = bidder_data[ "BidderID_int" ]
		
		# �p�G�S��P�� ���N�O��§���
		search_key = { "BidderID_int": bidder_id_ori }
		if bidder_id_ori == bidder_id:
			self.__dbtable_buyer.update( search_key, bidder_data )
		
		# �p�G�n��P��
		else:
			# ���մ��J��� �p�G���ѥN����
			if not self.__add_buyer( bidder_data ):
				return
			
			# �R���µP�����
			self.__dbtable_buyer.remove( search_key )
			
		self.__gen_list_ui_by_bidder_data()
		self.__add_msg( Const[ "FIX_BIDDER_DONE" ] % self.__gen_bid_id_plus_name_str( bidder_id ) )
		
		index, bidder_data = self.__cached_data[ bidder_id ]
		self.components.ListBidders.selection = index
		
	# ���U �R���R�a��� ���s
	def on_ButtonDelBidder_mouseClick( self, event ):
		# �S�����������
		index, bidder_id = self.get_bidder_id_by_selection()
		if bidder_id not in self.__cached_data:
			self.__add_msg( Const[ "NO_SELECTED_BIDDER" ] )
			return
		
		# ���o�n��ܪ���r/index
		mag_arg = self.__gen_bid_id_plus_name_str( bidder_id );
		index, bidder_data = self.__cached_data[ bidder_id ]
		
		# �R���P�����
		self.__dbtable_buyer.remove( { "BidderID_int": bidder_id } )
		
		# ��s���
		index_the_last = self.__gen_list_ui_by_bidder_data()
		self.__add_msg( Const[ "DEL_BIDDER_SUCCESS" ] % mag_arg )
		
		if index > index_the_last:
			index = index_the_last
		
		self.components.ListBidders.selection = index
		self.__display_bidder_data( {} )
		
	# ���U �M�Ÿ�� ���s
	def on_ButtonClear_mouseClick( self, event ):
		self.__display_bidder_data( {} )
		
	# ���U �j�M ���s
	def on_ButtonSearch_mouseClick( self, event ):
		search_text = self.components.TextFieldSearch.text
		search_text = None if search_text == "" else search_text
		self.__gen_list_ui_by_bidder_data( search_text )
		
	# ���U �ץX ���s
	def on_ButtonExport_mouseClick( self, event ):
		wb = xlwt.Workbook()
		ws = wb.add_sheet( "bidders" )
		
		com = self.components
		index_row = 0
		for index, attr in enumerate( BIDDER_ATTRS ):
			ws.write( index_row, index, getattr( com, "StaticText%s" % attr ).text )
		index_row += 1
		
		for cached_data_iter in self.__cached_data.itervalues():
			collection = cached_data_iter[ 1 ]
			for index, attr in enumerate( BIDDER_ATTRS ):
				ws.write( index_row, index, collection[ attr ] )
				
			index_row += 1
			
		wb.save( EXPORT_PATH )
		self.__add_msg( Const[ "ERRMSG_EXPORT_OK" ] % EXPORT_PATH )
	
	# ��ܶR�a���
	def __display_bidder_data( self, bidder_data ):
		com = self.components
		for attr in BIDDER_ATTRS:
			getattr( com, "TextField" + attr ).text = bidder_data[ attr ] if attr in bidder_data else ""
			
		com.ComboBoxGuaranteeType.text = bidder_data[ "GuaranteeType" ] if "GuaranteeType" in bidder_data else ""
		if self.__login_key:
			com.TextFieldAuctioneer.text = self.__login_key
		
	# ���U�R�a�C����
	def on_ListBidders_select( self, event = None ):
		index, bidder_id = self.get_bidder_id_by_selection()
		if bidder_id not in self.__cached_data:
			if bidder_id > 0:
				self.__add_msg( Const[ "NO_BIDDER_DATA" ] % bidder_id )
			return
		
		index, bidder_data = self.__cached_data[ bidder_id ]
		self.__display_bidder_data( bidder_data )
	
	# �Q�ί��ޭȨ��o�P��ID
	def get_bidder_id_by_index( self, index ):
		if index in self.__chched_index:
			return self.__chched_index[ index ]
		
		return INVALID_ID
		
	# �Q�Τ���������o�P��ID
	def get_bidder_id_by_selection( self ):
		index = self.components.ListBidders.selection
		if index < 0:
			return index, INVALID_ID
		
		return index, self.get_bidder_id_by_index( index )
	
	# �Ѭɭ�����r��J�� ���ͶR�a���
	def gen_bidder_data_by_text_field( self ):
		# �@�򥻸���˴�
		com = self.components
		
		# ������Ƥ��i����
		bidder_data = {}
		for attr in BIDDER_ATTRS:
			chk_text = getattr( com, "TextField" + attr ).text
			if attr in NONEMPTY_ATTRS and not len( chk_text ):
				self.__add_msg( Const[ "TEXT_CHK_FAIL_EMPTY" ] % getattr( com, "StaticText" + attr ).text )
				return None
			
			bidder_data[ attr ] = chk_text
			
		bidder_data[ "GuaranteeType" ] = com.ComboBoxGuaranteeType.text
		
		bidder_id_str = bidder_data[ "BidderID" ]
		
		# �P���L�o�W�h(���i��4, 7)
		# re_ob = re.compile( "[47]+" )
		# if re_ob.search( bidder_id_str ):
			# self.__add_msg( Const[ "BIDDER_ID_HAS_FOUR" ] )
			# return None
			
		# �P�������O�¼Ʀr
		if not bidder_id_str.isdigit():
			self.__add_msg( Const[ "BIDDER_ID_NOT_DIGIT" ] )
			return None
		
		# �O�Ҫ�ú�ǥ����O�¼Ʀr
		if bidder_data[ "GuaranteeCost" ] == "":
			com.TextFieldGuaranteeCost.text = "0"
			bidder_data[ "GuaranteeCost" ] = "0"
		
		if not bidder_data[ "GuaranteeCost" ].isdigit():
			self.__add_msg( Const[ "GUARANTEECOST_NOT_DIGIT" ] )
			return None
		
		# ID�O���
		bidder_data[ "BidderID_int" ] = int( bidder_data[ "BidderID" ] )
		
		# �إ�PK
		bidder_data[ "_id" ] = bidder_data[ "BidderID_int" ]
		return bidder_data
		
	# �̶R�a��Ʋ��ͬɭ����
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
		
	# ���͵P��:�m�W���r��
	def __gen_bid_id_plus_name_str( self, bidder_id ):
		index, bidder_data = self.__cached_data[ bidder_id ]
		return "%s: %s" % ( bidder_data[ "BidderID" ], bidder_data[ "Name" ] )
		
	# �{�Ǭ���
	def __add_msg( self, msg, plus_pop = False ):
		if plus_pop:
			dialog.alertDialog( self, msg, Const[ "WARNING" ] )
		self.components.TextAreaLog.appendText( msg + "\n" )

	# �����r��s(�ɤO�k�C�X suck!)
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
