/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package createhotspot;

import java.io.*;
import java.net.*;
import java.util.Date;

/**
 *
 * @author Sab
 */
public class CreateHotspot {

  
    public static void main(String[] args) throws IOException {
        // TODO code application logic here
        try {
            Process p = Runtime.getRuntime().exec("cmd /c start CreatingHotspot.lnk");
            String msg_received="";
            while(!msg_received.equals("exit")){
            ServerSocket socket = new ServerSocket(1755);
            Socket clientSocket = socket.accept();   //This is blocking. It will wait.
            DataInputStream DIS = new DataInputStream(clientSocket.getInputStream());
            msg_received = DIS.readUTF();
            System.out.println(msg_received);
            clientSocket.close();
            socket.close();
            }

        } catch (IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        
        }

}
