using System;
using System.Linq;


//using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class SourceDestinationSubscriber : MonoBehaviour
{
    const int k_NumRobotJoints = 6;

    public static readonly string[] LinkNames =
        {"base_link/base_link_inertia/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link"};
    public static readonly string[] JointNames =
        {"shoulder_pan_joint", "shoulder_lift_joint", "elbow_joint", "wrist_1_joint", "wrist_2_joint", "wrist_3_joint"};

    [SerializeField]
    string m_TopicName = "/move_group/fake_controller_joint_states";

    [SerializeField]
    GameObject m_UR5e;
    
    // Robot Joints
    ArticulationBody[] m_JointArticulationBodies;
    // ROS Connector
    ROSConnection m_Ros;

    void Start(){
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();

        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++){
            linkName += LinkNames[i];
            print(linkName);
            m_JointArticulationBodies[i] = m_UR5e.transform.Find(linkName).GetComponent<ArticulationBody>();
        }
        ROSConnection.GetOrCreateInstance().Subscribe<JointStateMsg>(m_TopicName, CommandCallback);
    }
    
    void CommandCallback(JointStateMsg jointMessage){
        print(jointMessage);
        var jointPosition = jointMessage.position;
        var position_array = jointPosition.Select(r => (float)r * Mathf.Rad2Deg).ToArray();
        
        var joint_array = position_array.Zip(jointMessage.name, (position, name) => new {Position = position, Name = name});
        foreach (var i in joint_array){
            print(i.Name);
            print(i.Position);
            int ind = Array.IndexOf(JointNames, i.Name);

            var joint1XDrive = m_JointArticulationBodies[ind].xDrive;
            joint1XDrive.target = i.Position;
            m_JointArticulationBodies[ind].xDrive = joint1XDrive;
        }
        print("packddddddddddddddddddddddddddd");
        //new WaitForSeconds(0.5f);
    }
}
