using System;

//using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class SourceDestinationPublisher : MonoBehaviour
{
    const int k_NumRobotJoints = 6;

    public static readonly string[] LinkNames =
        {"base_link/base_link_inertia/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link"};
    public static readonly string[] JointNames =
        {"shoulder_pan_joint", "shoulder_lift_joint", "elbow_joint", "wrist_1_joint", "wrist_2_joint", "wrist_3_joint"};

    // Variables required for ROS communication
    [SerializeField]
    string m_TopicName = "/joint_states";

    [SerializeField]
    GameObject m_UR5e;

    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);

    // Robot Joints
    UrdfJointRevolute[] m_JointArticulationBodies;

    // ROS Connector
    ROSConnection m_Ros;

    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<JointStateMsg>(m_TopicName);

        m_JointArticulationBodies = new UrdfJointRevolute[k_NumRobotJoints];

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += LinkNames[i];
            print(linkName);
            m_JointArticulationBodies[i] = m_UR5e.transform.Find(linkName).GetComponent<UrdfJointRevolute>();
        }
    }

    // public void Publish()
    void Update()
    {
        var sourceDestinationMessage = new JointStateMsg();
        sourceDestinationMessage.name = JointNames;
        double []position = new double[JointNames.Length];
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            //sourceDestinationMessage.joints[i] = m_JointArticulationBodies[i].GetPosition();
            try{
                print(LinkNames[i] + ", " + JointNames[i]);
                print(m_JointArticulationBodies[i].GetPosition());
                position[i] = m_JointArticulationBodies[i].GetPosition();
                print("---------------------------------------------------");
            }catch{}
        }
        sourceDestinationMessage.position = position;
        // Finally send the message to server_endpoint.py running in ROS
        m_Ros.Publish(m_TopicName, sourceDestinationMessage);
    }
}


/* msg format
---
header: 
  seq: 95763
  stamp: 
    secs: 191
    nsecs: 858000000
  frame_id: ''
name: 
  - elbow_joint
  - shoulder_lift_joint
  - shoulder_pan_joint
  - wrist_1_joint
  - wrist_2_joint
  - wrist_3_joint
position: [-1.5707958774411832, -0.7539701852810134, -0.00018470964782490995, 3.67253865274364e-06, 2.5263412732456914e-05, 6.403013947497982e-06]
velocity: [0.00044935371522482163, 0.012051580562754698, 0.0007240410412547526, -0.0005212095306967712, -0.00020492686381269918, 0.00011074915592053716]
effort: [0.0, 0.0, 0.0, 0.0, 0.0, 0.0]
---
*/
