   M S B A M L   `   `   `  ����H  DIRC_Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:IRC_InterfaceIRC_Interface  5   	       IRC_Interface.InitWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:IRC_Interface     5   	     � Title$  Configure the App��5   	   $��258.27��6#   $	��300��63    � 
ResizeMode$ NoResize=�6?   .��5	      � .+�5   
   �� -   5         ��Name$ StartButton��5      $��Start��6&   $
��Left=�66   $��209,198,0,0q�6Q   $	��Top=�6f   $��75��6~   6�   �� 5   
   -   5        $ CancelButton��5      $��Cancel��6'   $
��Left=�68   $��129,198,0,0q�6S   $	��Top=�6h   $��75��6�   6�   �� 5   
   -   5         ��Name$ groupbox��5      $��	App Type:��6%   $
��Left=�68   $��	10,10,0,0q�6S   $	��Top=�6f   .��5      � .+�5      	� -   5         	�Name$ ServerSelect��5      $��Server��64   $
��Left=�6E   $��5,5,0,0q�6`   $	��Top=�6q   $��2��6�   	� 5      -   5        $ ClientSelect��5      $��Client��64   $
��Left=�6E   $��5,25,0,0q�6`   $	��Top=�6r   $��2��6�   5      5      �� 5   
   -   5        $ groupboxServer��5      $��Server Config:��6+   $
��Left=�6C   $��	80,10,0,0q�6^   $	��Top=�6q   $��63��6�   $	��204��6�   .��5      � .+�5      �� $��Listen Port:��6   $��0,0,0,0q�6/   �� 5      -   5         ��Name$ 
serverPort��5      $
��Left=�6.   $��23��6I   $��75,4,0,0q�6U    �� TextWrapping$
 Wrap=�6g   $
��1337��6{   $	��Top=�6�   $	��107��6�   5      5      �� 5   
   -   5        $ groupboxClient��5      $��Client Config:��6+   $
��Left=�6C   $��	10,78,0,0q�6^   $	��Top=�6q   $	��274��6�   .��5      � .+�5      �� $��
Dest Port:��6   $��0,0,0,0q�6-   �� 5      -	   5        $ 
clientPort��5      $
��Left=�6.   $��23��6I   $��75,4,0,0q�6U   $
 Wrap=�6g   $
��1337��6{   $	��Top=�6�   $	��177��6�   �� 5       $��
Dest Addr:��6   $��0,35,0,0q�6-   �� 5!      -
   5        $ 
clientAddr��5!      $
��Left=�6.   $��23��6I   $��	75,39,0,0q�6U   $
 Wrap=�6h   $��	localhost��6|   $	��Top=�6�   $	��177��6�   �� 5"      $��	Nickname:��6   $��0,65,0,0q�6,   �� 5#      -   5        $ 
clientNick��5#      $
��Left=�6.   $��23��6I   $��	75,69,0,0q�6U   $
 Wrap=�6h   $��sabreman��6|   $	��Top=�6�   $	��177��6�   5$      5%      5(      5)      