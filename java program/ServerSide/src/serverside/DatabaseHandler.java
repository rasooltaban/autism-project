/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package serverside;

import java.io.IOException;
import java.sql.*;
import java.util.HashMap;
import java.util.Map;
/**
 *
 * @author Sab
 */
public class DatabaseHandler {
     // JDBC driver name and database URL
   static final String JDBC_DRIVER = "com.mysql.jdbc.Driver";  
   static final String DB_URL = "jdbc:mysql://localhost:3306/utautismapplication?zeroDateTimeBehavior=convertToNull";

   //  Database credentials
   static final String USER = "root";
   static final String PASS = "123456";
   Connection conn = null;
   Statement stmt = null;
   
   DatabaseHandler() {
   try{
      //STEP 2: Register JDBC driver
      Class.forName("com.mysql.jdbc.Driver");

      //STEP 3: Open a connection
      System.out.println("Connecting to a selected database...");
      conn = DriverManager.getConnection(DB_URL, USER, PASS);
      System.out.println("Connected database successfully...");
      

   }catch(SQLException se){
      //Handle errors for JDBC
      se.printStackTrace();
   }catch(Exception e){
      //Handle errors for Class.forName
      e.printStackTrace();
   }
}//end main
   
   public void ExecuteTheInsertQuery(String command,String id,String firstName, String lastName, String age, String gender, String phoneNumber){
       //STEP 4: Execute a query
       try{
           if(command.equals("insert")){
            System.out.println("Inserting records into the table...");
            stmt = conn.createStatement();
            String sql = "INSERT INTO patient " +
                   "VALUES ( '"+id+"','"+firstName+"', '"+lastName+"', '"+age+"', '"+gender+"', '"+phoneNumber+"', '"+0+"', '"+0+"', '"+0+"', '"+0+"', '"+0+"')";
            stmt.executeUpdate(sql);
            System.out.println("Inserted records into the table...");
           }
           
       }catch(SQLException se){
           se.printStackTrace();
       }
   }
   public Map<String,Double> ExecuteTheSelectQuery(String command, String firstName, String lastName, String phoneNumber)throws SQLException{
       Map<String,Double> Results = new HashMap<>();
       if(command.equals("select")){
               System.out.println("Selecting Record from the table ...");
               stmt = conn.createStatement();
               String sql = "SELECT * FROM patient p WHERE "+
                       "p.FirstName LIKE '%"+firstName+"%' AND p.LastName LIKE '%"+lastName+"%' AND p.Phone LIKE '%"+phoneNumber+"%'";
                ResultSet rs = stmt.executeQuery(sql);
                while(rs.next()){
                    System.out.println("Results:");
                    System.out.println(rs.getString("FirstName")+" "+rs.getString("LastName")+" "+rs.getString("Age")+" "+rs.getString("gender")+" "+rs.getString("Email")+" "+rs.getString("Phone")+" "+rs.getString("KinectResult")+" "+rs.getString("CarResult")+" "+rs.getString("OverheadCameraResult")+" "+rs.getString("ParrotResult")+" "+rs.getString("VoiceResult"));
                    Results.put("KinectResult", rs.getDouble("KinectResult"));
                    Results.put("CarResult", rs.getDouble("CarResult"));
                    Results.put("ParrotResult", rs.getDouble("ParrotResult"));
                    Results.put("OverheadCameraResult", rs.getDouble("OverheadCameraResult"));
                    Results.put("VoiceResult", rs.getDouble("VoiceResult"));
                }
               
        }
       return Results;
   }
   public void ExecuteTheUpdateQuery(String command, String firstName, String lastName, String phoneNumber, String attributeToChange, Double newValue)throws SQLException{
       if(command.equals("update")){
           System.out.println("Updating one the value of "+ attributeToChange+" ...");
           stmt = conn.createStatement();
           String sql = "UPDATE patient p SET "+ attributeToChange+"= "+newValue+" WHERE "+
                       "p.FirstName ='"+firstName+"' AND p.LastName ='"+lastName+"' AND p.Phone = '"+phoneNumber+"'";
           stmt.executeUpdate(sql);
           System.out.println("The value of "+attributeToChange+" changed to "+newValue);
       }
   }
}
