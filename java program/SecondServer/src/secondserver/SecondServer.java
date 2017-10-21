/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package secondserver;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;

/**
 *
 * @author Sab
 */
public class SecondServer {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws IOException {
        ConnectToWifi();
        try {
            //DataOutputStream DOS = new DataOutputStream(socket.getOutputStream());
            String MessageFrom1To2 = "";
            while (!MessageFrom1To2.equals("exit")) {
                Socket socket = new Socket("192.168.173.1", 2035);
                DataInputStream DIS = new DataInputStream(socket.getInputStream());
                MessageFrom1To2 = DIS.readUTF();
                System.out.println(MessageFrom1To2);
                socket.close();
            }
        }catch(IOException ie){
            ie.printStackTrace();
        }

    }

    public static void ConnectToWifi() throws IOException {
        Runtime.getRuntime().exec("cmd /c start ConnectToHostspot.lnk");
    }
    public static void StartParrot(){
        
    }
    public static void StopParrot(){
        
    }
    public static void StartOverheadCamera(){
        
    }
    public static void StopOverheadCamera(){
        
    }

}
