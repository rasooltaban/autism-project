   M S B A M L   `   `   `  ����   WpfApplication41    +UTKinectSkeletonMovementDetector.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$0  *UT-Autism Detection Using Microsoft Kinect�� � SizeToContent$ WidthAndHeight=� � WindowStyle$ SingleBorderWindow=� � 
ResizeMode$ CanResizeWithGrip=� � WindowState$ 	Maximized=� � WindowStartupLocation$ CenterOwner=�.��� -   $��mainGrid��
�������$	��400��$	��800�� � RowDefinitions �� $��30 ��� $��20 ��� $��3* ��� $��20 ��� $��3* ��� $��30 ��� $��17 ��� $	��150 ��� $��20 ��� $��30 � � ColumnDefinitions �� $��20 ��� $��4* ��� $��20 ��� $��8* ��� $��20 ��� $��4* ��� $��20 �.+�o� -   $��	main_menu��$��0��$��0��$��7��$��30��.@�m� $��_File��$��30��$��50��.@�m� -   $��newProjectItem��$��_New Project��m� -   $��openProjectItem��$��_Open Project���� m� -   $��exitItem��$��_Exit���� $��1��$��2��$,��&/WpfApplication4;component/MIRlogo.png�� J� RenderTransformOrigin$ 0.282,0.872&�$
��Left=�$	��207���� $��5��$��2��$+��%/WpfApplication4;component/UTlogo.png���� -   $��overlayBackground��
�������$��2��$��3��$��3���� -   $��ColorImage2��$��3��$��2��$��3���� -	   $��	Skeleton2��$��3��$��2��$��3��$��Bottom=�$	��271��$	��361��r� -
   $��overlayplayer��$��2��$��3��$��3��	 r� LoadedBehavior$	 Manual=�
 r� ScrubbingEnabled
 . �� -   $��OverlayTextLabel��$!��>  RGB and Skeleton Overlay��$��16(�
�������$��2��$��3�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 0Q�   
BlurRadius$ 10Q�   Color$ 	#00000000���� -   $��colorImageLabel��
�������$��4��$��1���� -   $��
ColorImage��$��4��$��1��r� -   $��	rgbplayer��$��4��$��1��$	 Manual=�
 . �� -   $��colorTextLabel��
�������$��>  RGB Data��$��16(�$��4��$��1��   $ 0Q�$ 10Q�$ 	#00000000���� -   $��skeletonLabel��
���   �$��4��$��5���� -   $��Skeleton��$��4��$��5��r� -   $��skeletonplayer��$��4��$��5��$	 Manual=�
 . �� -   $��skeletonTextLabel��$��16(�$��4��$��5���� 
�������.�> Skeleton Data� -   $��buttonsGrid��
�������$��1��$��7�� �� $��40 ��� $��40 ��� $��40 � �� $��* ��� $��* �.+��� -   $��live_button��$��0��$��0��$��
5, 5, 5, 5q�.��	Go Live�� -   $��record_button��$��1��$��0��$��
5, 5, 5, 5q�.��Record�� -   $��caseMaker_button��$��1��$��0��$��
5, 5, 5, 5q�.��
Case Maker�� -   $��caseSelector��$��0��$��1��$��
5, 5, 5, 5q��� -   $��play_pause_button��$��1��$��1��$��
5, 5, 5, 5q�.��Play�� -   $��newTagButton��$��0��$��2��$��
5, 5, 5, 5q�.��	New Tag�� -   $��tagLockButton��$��1��$��2��$��
5, 5, 5, 5q�.��
Unlock Tag�� -   $��	statusBar��$��10��$��0��$��9��$��30��.@��� .���� -   $��status_text��� -   $��liveConsoleGrid��$��7��$��3��$��3��
����iA� �� $��25 ��� $��25 ��� $��25 ��� $��25 � �� $	��160 ��� .+��� $��0��$��0��
�������.��Live Observation :�� $��1��$��0��
�������.��Recorded RGB Frames :�� $��2��$��0��
�������.��Recorded Skeleton Frames :�� -   $��clabel1��$��0��$��1��
�������.��false�� -    $��clabel2��$��1��$��1��
�������.��!�� -!   $��clabel3��$��2��$��1��
�������.��!� -"   $��offlineConsoleGrid��$��7��$��3��$��3��.+�&   System.Windows.Controls.DataGrid  -#   $��tagsDataGrid��   AutoGenerateColumns .  $��
0, 5, 0, 0q� J� IsHitTestVisible .     Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$
 Note��   
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ * 1  +System.Windows.Controls.DataGridBoundColumn   Binding ��  �� Path$
 text��� -$   $��slider��$��3��
�������$��0, 10, 0, 0q�$��5��$��1��� -%   $��	tagsPlace��$��6��$��3���� $��1��$��183,26,10,50q�$��2��$��CTADlogo.png��$
��Fill=� � 	Resources )  #System.Windows.Controls.DataGridRow �� 
TargetType �� Triggers �� Binding �� Value �� Property �� Value��%�  '        �� "   ��  �� *	visible+ true.a���  � y�$ Visible=���  �� *	visible+	 false.a���  � y�$ 	Collapsed=���  �� *color+ 0.a���  � ��
 �Gc����  �� *color+ 1.a���  � ��
 ��·���  �� *color+ 2.a���  � ��
 �2�2���  �� *color+ 3.a���  � ��
 � �����  �� *color+ 4.a���  � ��
 � ���