using System;

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
    string m_TopicName = "/pos_joint_traj_controller/command";

    [SerializeField]
    GameObject m_UR5e;
    
    // Robot Joints
    UrdfJointRevolute[] m_JointArticulationBodies;
    // ROS Connector
    ROSConnection m_Ros;

    void Start(){
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<JointStateMsg>(m_TopicName);

        m_JointArticulationBodies = new UrdfJointRevolute[k_NumRobotJoints];

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++){
            linkName += LinkNames[i];
            print(linkName);
            m_JointArticulationBodies[i] = m_UR5e.transform.Find(linkName).GetComponent<UrdfJointRevolute>();
        }
        ROSConnection.GetOrCreateInstance().Subscribe<JointStateMsg>(m_TopicName, CommandCallback);
    }
    
    void CommandCallback(JointStateMsg jointMessage){
        print(jointMessage);
    }
}
