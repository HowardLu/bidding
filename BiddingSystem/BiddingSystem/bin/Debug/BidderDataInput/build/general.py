#-*- coding: utf-8 -*-

import sys, traceback

def run_main_process( main ):
	try:
		main()
	except:
		exc_info = sys.exc_info();
		print exc_info[ 0 ]
		print exc_info[ 1 ]
		traceback.print_tb( exc_info[ 2 ] )
		
	raw_input( "press [Enter] to end..." )
