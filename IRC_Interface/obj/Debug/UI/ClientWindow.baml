   M S B A M L   `   `   `  ����H  DIRC_Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:IRC_InterfaceIRC_Interface  5   	        IRC_Interface.ClientWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:IRC_Interface     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5	      � .+�5
   
   �� -   5         ��Name$ 
ChatTarget��5
       �� TextWrapping$
 Wrap=�6&   $��Chatting With: ��6:   $��
88,10,88,0q�6Q   $��10(�6e   $��Center=�5      $��0,-2,0,0q�64   $��18��6G   $	��Top=�6S    �� 
IsReadOnly . 6k   �� 5   
   -   5        $ ChatBox��5      $��23��6#   $��
10,0,88,10q�6/   $
 Wrap=�6C   $�� ��6W   $��Bottom=�6_   6z   �� 5   
   -   5         ��Name$ 	SubmitMSG��5      $
��Send��6$   $��Center=�63   $��Center=�6W    �� TextAlignment$ Center=�6y   $��Right=�6�   $��	0,0,10,10q�5      $��Bottom=�6$   $��73��6?   $��23��6J   6V   �� 5   
   -   5         ��Name$ NickList��5      $��Right=�6%   $��
0,10,10,38q�6A   $��73��6U   6`   �� 5   
   -   5        $ channelList��5      $
��Left=�6(   $��
10,10,0,38q�6C   $��73��6W   6b   �� 5   
   -   5         ��Name$ 
MessageBox��5      $��88,28,88,38q�6*    . " �� VerticalScrollBarVisibility$
 Auto=�6v   � 5   
   -   5        $��ConnectionOverlay��5      
�������	 J� Opacity$		 0.8Q�6:   
 � RowDefinitions
 5      �� 5      $��* �6    �� 5      $��* �6    .+�5      �� $��Connecting to Server��6   $��0��63   $��Center=�6@   $��Center=�6[   $��45(�6x   �� 5      $��Please Wait��6   $��0��6*   $��	5,100,5,0q�67   $��Center=�6J   $��25(�6g   � 5       � IsIndeterminate . $��1��61   $��5q�6>   $��25��6I   5      5      5      