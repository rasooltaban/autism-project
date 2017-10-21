/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package createhotspot;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.ServerSocket;
import java.net.Socket;

/**
 *
 * @author Sab
 */
public class client_1 {
    public static void main(String[] args) throws IOException {
        // TODO code application logic here
       
            //Process p = Runtime.getRuntime().exec("cmd /c start CreatingHotspot.lnk");
            //ServerSocket listener = new ServerSocket(9090);
//            String msg_received;
//            ServerSocket socket = new ServerSocket(1755);
//            Socket clientSocket = socket.accept();       //This is blocking. It will wait.
//            DataInputStream DIS = new DataInputStream(clientSocket.getInputStream());
//            msg_received = DIS.readUTF();
//            clientSocket.close();
//            socket.close();
//            Socket socket = new Socket(,9000);
//            DataOutputStream outputStream = new DataOutputStream(socket.getOutputStream());
//            BufferedReader inputStream = new BufferedReader(new InputStreamReader(socket.getInputStream()));
//
//            String message = inputStream.readLine();
//            System.out.println(message);                
//            //in server
//            String txt = "Hello from client to server\n";           
//            outputStream.write(txt.getBytes());
//            
//            
//            socket.close();
//            Socket socket = new Socket("192.168.1.5",1755);
//            DataOutputStream DOS = new DataOutputStream(socket.getOutputStream());
//            DOS.writeUTF("HELLO_WORLD");
//            socket.close();
//
//        } catch (IOException e) {
//            // TODO Auto-generated catch block
//            e.printStackTrace();
//        }
//        
//        }
        Runtime.getRuntime().exec("taskkill /F /IM WpfApplication4.exe");
        }
}
