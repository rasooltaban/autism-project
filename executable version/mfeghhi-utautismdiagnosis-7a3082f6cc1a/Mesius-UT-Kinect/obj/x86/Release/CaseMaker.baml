   M S B A M L   `   `   `  ����   WpfApplication40    *UTKinectSkeletonMovementDetector.CaseMaker   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  	CaseMaker��$	��670��$	��680�� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=�.��� -   $��mainGrid��
������� � RowDefinitions �� $��20 ��� $	��480 ��� $��30 ��� $��20 ��� $��73 ��� $��20 ��� $	��69* � � ColumnDefinitions �� $��20 ��� $	��640 ��� $��20 ��� $��2* �.+��� -   $��timeTagLabel��$��
-Time Tag-��$��0��$��10��$��2��$��1��$��30��$��Center=�$	��Top=��� -   $��slider��$��0��$��10�� � Maximum$	 100Q�$��3��$��2��$��20��$��Center=�$	��Top=�$	��500���� $��Case Start :��$��1��$��4��$��28��$
��Left=�$��	70,10,0,0q�$	��Top=��� $��Case End  :��$��1��$��4��$��28��$
��Left=�$��	70,40,0,0q�$	��Top=��� -   $��caseStartLabel��$��
Not Marked��$��1��$��4��$��28��$
��Left=�$��
140,10,0,0q�$	��Top=��� -   $��caseEndLabel��$��
Not Marked��$��1��$��4��$��28��$
��Left=�$��
140,40,0,0q�$	��Top=��� -   $��markStartButton��$��
Mark Start��$��1��$��4��$��23��$
��Left=�$��
220,10,0,0q�$	��Top=�$��75���� -   $��markEndButton��$��Mark End��$��1��$��4��$��23��$
��Left=�$��
220,40,0,0q�$	��Top=�$��75���� -   $��
makeButton��$
��Make��$��1��$��4��$��53��$
��Left=�$��
489,10,0,0q�$	��Top=�$��75���� -	   $��makeSliderOneValueBigger��$��>��$��1��$��4��$��23��$
��Left=�$��
342,11,0,0q�$	��Top=�$��23���� -
   $��makeSliderOneValueLittler��$��<��$��1��$��4��$��23��$
��Left=�$��
314,11,0,0q�$	��Top=�$��23���� $��1��$��1��$	��480��$	��640��$
��Left=�
�������$	��Top=��� -   $��currentFrame��$��1��$��3��$��1��$	��480��$	��640��$
��Left=�$
��Fill=�$	��Top=�