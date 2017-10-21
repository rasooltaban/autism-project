/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package serverside;

import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;
import java.sql.SQLException;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.TimeUnit;

/**
 *
 * @author Sab
 */
public class ServerSide {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) throws IOException, SQLException {
        ConnectToWifi();
        DatabaseHandler DH = new DatabaseHandler();
        //DH.ExecuteTheUpdateQuery("update", "Parsa", "akbari","09354268775","KinectResult",12.51);
        String fileName = "C:\\Users\\RoboLab\\Documents\\autism project\\executable version\\mfeghhi-utautismdiagnosis-7a3082f6cc1a\\Mesius-UT-Kinect\\bin\\CheckStopStartStatus.txt";
        File file = new File(fileName);
        file.createNewFile();
        try {
            String msg_received = "";
            ServerSocket socket1 = new ServerSocket(1755);
            ServerSocket socket2 = new ServerSocket(3025);
            String childId = null;
            //StartKinect("sahar", "mardani");
            while (!msg_received.equals("exit")) {
                Socket clientSocket = socket1.accept();   //This is blocking. It will wait.
                DataInputStream DIS = new DataInputStream(clientSocket.getInputStream());
                DataOutputStream DOS = new DataOutputStream(clientSocket.getOutputStream());
                msg_received = DIS.readUTF();
                String[] SpliteMsg = msg_received.split("\\s+");
                //ConnectServer1ToServer2(socket2, "hi");
                switch (SpliteMsg[0]) {
                    case "Enter":
                        childId = generateId();
                        System.out.println("generated Child id is: " + childId);
                        DH.ExecuteTheInsertQuery("insert", childId, SpliteMsg[1], SpliteMsg[2], SpliteMsg[3], SpliteMsg[4], SpliteMsg[5]);
                        System.out.println(SpliteMsg[1] + " " + SpliteMsg[2] + " " + SpliteMsg[3] + " " + SpliteMsg[4] + " " + SpliteMsg[5] + " ");
                        StartKinect(SpliteMsg[1], SpliteMsg[2]);
                        StartCar(childId);
                        //ConnectServer1ToServer2(socket2, "StartCamera" + " " + childId);
                        //DOS.writeUTF("Kinect and Car Start");
                        //DOS.flush();
                        break;
                    case "StartParrot":
                        System.out.println("try to start parrot");
                        ConnectServer1ToServer2(socket2, "StartParrot" + " " + childId);
                        break;
                    case "EndParrot":
                        System.out.println("try to stop parrot");
                        ConnectServer1ToServer2(socket2, "EndParrot");
                        break;
                    case "EndProgram":
                        System.out.println("try to stop all program");
                        StopKinect(fileName);
                        ConnectServer1ToServer2(socket2, "StopDevices");
                        break;
                    case "ShowResults":
                        Map<String, Double> DeviceResults = DH.ExecuteTheSelectQuery("select", SpliteMsg[1], SpliteMsg[2], SpliteMsg[3]);
                        String aggregateResult = "";
                        for (Map.Entry<String, Double> entry : DeviceResults.entrySet()) {
                            aggregateResult += entry.getValue() + " ";
                            System.out.println(entry.getKey() + "/" + entry.getValue());
                        }
                    //DOS.writeUTF(aggregateResult);
                    //DOS.flush();
                }
                clientSocket.close();
            }
            socket1.close();
            socket2.close();
        } catch (IOException ie) {
            ie.printStackTrace();
        }
    }

    /**
     * This function create a hotspot and start it as an admin
     */
    private static void CreateHotspot() throws IOException {
        Runtime.getRuntime().exec("cmd /c start CreatingHotspot.lnk");
    }

    /**
     * This function stop a hotspot as an admin
     */
    private static void StopHotspot() throws IOException {
        Runtime.getRuntime().exec("cmd /c start StopHotspot.lnk");
    }

    /**
     * This function start the kinect device
     */
    private static void StartKinect(String firstName, String lastName) throws IOException {
        System.out.println("Start kinect");
        System.out.println("first Name:"+firstName+" lastName:"+lastName);
        //Process p = new ProcessBuilder("C:\\Users\\RoboLab\\Documents\\autism project\\executable version\\mfeghhi-utautismdiagnosis-7a3082f6cc1a\\Mesius-UT-Kinect\\bin\\Release\\WpfApplication4.exe", "/" + firstName, "/" + lastName).start();
        //System.out.println(".....");
        //String[] args = {"/" +firstName,"/" +lastName};
        //Runtime.getRuntime().exec("C:\\Users\\RoboLab\\Documents\\autism project\\executable version\\mfeghhi-utautismdiagnosis-7a3082f6cc1a\\Mesius-UT-Kinect\\bin\\Release\\WpfApplication4.exe", null, new File("C:\\Users\\RoboLab\\Documents\\autism project\\executable version\\mfeghhi-utautismdiagnosis-7a3082f6cc1a\\Mesius-UT-Kinect\\bin\\Release\\"));
        String command = "C:\\Users\\RoboLab\\Documents\\autism project\\executable version\\mfeghhi-utautismdiagnosis-7a3082f6cc1a\\Mesius-UT-Kinect\\bin\\Release\\WpfApplication4.exe";
        Runtime.getRuntime().exec(new String[]{command,"/"+firstName,"/"+lastName},null,new File("C:\\Users\\RoboLab\\Documents\\autism project\\executable version\\mfeghhi-utautismdiagnosis-7a3082f6cc1a\\Mesius-UT-Kinect\\bin\\Release\\"));
    }

    private static String generateId() {
        Random randomGenerator = new Random();
        int randomInt = randomGenerator.nextInt(100000);
        String gId = String.valueOf(randomInt);
        return gId;
    }

    /**
     * This function start the car robot
     */
    private static void StartCar(String childID)throws IOException {
        System.out.println("try to start car");
        Runtime.getRuntime().exec("cmd /c start http://localhost:8080/car/getID.php?ID="+childID);
        System.out.println("car start sucessfully...");

        
    }

    /**
     * This function stop the kinect device
     */
    private static void StopKinect(String fileName) throws IOException {
        String content = "Stop";
        System.out.println("try to write stop in file");
        FileWriter fw = new FileWriter(fileName);
        BufferedWriter bw = new BufferedWriter(fw);
        bw.write(content);
        System.out.println("Write Stop In File");
        bw.close();
        fw.close();
        //TimeUnit.SECONDS.sleep(1);
        //Runtime.getRuntime().exec("taskkill /F /IM WpfApplication4.exe");
    }

    /**
     * This function connect server1 to server2 and send message to server2
     */
    private static void ConnectServer1ToServer2(ServerSocket socket, String message) throws IOException {
        try {
            Socket secondServerSocket = socket.accept();
            System.out.println("successfully connected to second server and try to send message: "+message);
            //This is blocking. It will wait.
            DataInputStream DIS = new DataInputStream(secondServerSocket.getInputStream());
            DataOutputStream DOS = new DataOutputStream(secondServerSocket.getOutputStream());
            DOS.writeUTF(message);
            DOS.flush();
            secondServerSocket.close();
        }catch(IOException e){
            e.printStackTrace();
        }
    }

    public static void ConnectToWifi() throws IOException {
        Runtime.getRuntime().exec("cmd /c start ConnectToWifi.lnk");
    }
}
