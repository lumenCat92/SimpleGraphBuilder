# LumenCat92
<div align="center">

![LumenCat92.jpg](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/LumenCat92.jpg)

본 작업은 유니티 엔진을 대상으로 합니다.  
this work target to Unity Engine.
</div>

# SimpleGraphBuilder
![Whatadis.jpg](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Whatadis.jpg)

# Language
<details>
<summary>English</summary>

# How Can Install This?

This supports installing packages through the Unity Package Manager using Git addresses.(Recommand)
![How_To_Install.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/How_To_Install.gif)

or U Can also just Download this to Assets Folder in your unity project.

# What is This?

![Whatadis.jpg](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Whatadis.jpg)
This tool allows you to effortlessly create graph structures based on your custom designs. You can easily define graphs, nodes, and the various connection types between those nodes.

# Where Can Use This?

To Every visualize connection between object to object.

# Why Should Use This?

There is a problem that working with object connections using tools like Excel is not intuitive.
This project aims to enable users to easily create graph structures and visualize object connections, and to make it easy to create other custom functions by understanding the structure of Node, NodeGraph, and NodeConnection.

# How to Use This?

1. Create a NodeGraph: 
    Go to "Create" -> "NodeGraph" -> "NodeGraph" to generate a new NodeGraph asset in your project.
![1_MakingGraph](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/1_MakingGraph.gif)


2. Define Connection Types: 
    Create a Child NodeConnection class and use an enum to define the possible connection types between your nodes. (It is required to use enum instead of string for safe automation.)
![2_Open_NodeConnection.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/2_Open_NodeConnection.gif)

    (Refer to the sample code provided in this project).
![3_define_new_ConnectionType.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/3_define_new_ConnectionType.gif)


3. Create a Connection Type ScriptableObject: 
    Add your connection types to a ConnectionType ScriptableObject and assign it to your NodeGraph.
![4_make_newConnectionTypeObj.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/4_make_newConnectionTypeObj.gif)
    * Important: If you add new connection types that node dosent had, existing node connections using those types might be lost. (To protect critical data, the existing NodeGraph will copied.)
![5_regist_ConnectionType_to_NodeGraph.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/5_regist_ConnectionType_to_NodeGraph.gif)


5. Launch SimpleGraphBuilderView: 
    Access the SimpleGraphBuilderView from "Window" -> "UI Toolkit" -> "SimpleGraphBuilder".
![6_Open_SimpleGraphBuilder.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/6_Open_SimpleGraphBuilder.gif)


6. Select Your NodeGraph: 
    Choose the NodeGraph object within the SimpleGraphBuilder.
7. Create the Root Node: 
    Right-click and select "Create Root Node" (Nodes can only be deleted from within SimpleGraphBuilderView).
8. Create Additional Nodes: 
    Right-click and select "Create Node".
9. Make Connection Between Node: 
    you Can Connect Each Node By Drag & Drop From Any Node's In/OutPut Port.
![7_Make_Node.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/7_Make_Node.gif)


# Any Useful Function Provide?
1. When u Click the Node from SimpleGraphBuilder, it will highligt connected Node in Poject.
![Func1_Highlight.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Func1_Highlight.gif)

2. When u Drag & Drop Object To objectField In Node In graphBuild, object will registed to connectionObj in Node.  
![Func2_Auto_Connect.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Func2_Auto_Connect.gif)

3. it support to UnDo & ReDo (ctrl + z or ctrl + y)
![Func3_Un%26ReDo.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Func3_Un%26ReDo.gif)

4. it provide finding path that target to root in 2 different way. 

```csharp
NodeGraph.FindPathWithMixConnectionType(Node currentNode, List<string> findConnectionType, ref List<List<Node>> pathNodes, ref List<Node> path, ref bool isPathDone, bool shouldDoneWithFirstFinding);
```
= This function will Return to find paths within the NodeGraph that contain a mixture of the connection types specified in findConnectionType.

```csharp
NodeGraph.FindPathWithEachConnectionType(Node currentNode, List<string> findConnectionType);
```
= This function will Return locate paths within the NodeGraph where each connection type from the findConnectionType list is used at least once along the path.(HighCost)
</details>

<details>
<summary>한국어</summary>

# 어떻게 설치하죠?

깃주소를 사용하여 유니티 패키지를 통해 설치 가능합니다.(추천)
![How_To_Install.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/How_To_Install.gif)

아니면 직접 다운로드해서 프로젝트의 Assets에 설치해도 됩니다.

# 용도는?

![Whatadis.jpg](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Whatadis.jpg)
오브젝트간의 연결을 시각화한 커스텀 그래프 구조를 만드는 것에 목표를 두고 있습니다.

# 어디에 쓰나요?

오브젝트간의 연결에 시각화가 필요한 모든 곳이라 정의할 수 있습니다.

# 왜 써야할까요?

오브젝트간의 연결을 엑셀과 같은 툴로 작업시 직관적이지 못하다는 문제가 있습니다.
해당 프로젝트는 사용자가 쉽게 그래프 구조를 생성하고 오브젝트간의 연결을 시각화 할 수 있게 하며, Node와 NodeGraph, NodeConnection 구조의 파악으로 다른 커스텀 기능을 쉽게 생성 가능합니다. 

# 어떻게 사용하나요?

1. NodeGraph 만들기: 
    유니티에서 "Create" -> "NodeGraph" -> "NodeGraph"를 통해 새로운 NodeGraph를 생성한다.
![1_MakingGraph](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/1_MakingGraph.gif)


2. 새로운 자식 NodeConnection 클래스 생성: 
    노드간의 연결 타입을 나타낼 NodeConnection의 자식 클래스를 생성한다. (안전한 자동화를 위해 string대신 enum을 사용하도록 되어 있음) 
![2_Open_NodeConnection.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/2_Open_NodeConnection.gif)

    (샘플이 제공됨으로 참고해서 사용할 것)
![3_define_new_ConnectionType.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/3_define_new_ConnectionType.gif)


3. NodeConnection ScriptableOject 생성하고 연결하기: 
    새로 만든 NodeConnection ScriptableOject를 생성하고, 사용한 NodeGraph에 연결하기.
![4_make_newConnectionTypeObj.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/4_make_newConnectionTypeObj.gif)
    * 중요: NodeGraph에 있는 기존 Node들의 ConnectionType이, 새로운 NodeConnection에서 지원하지 않는다면, 해당 ConnectionType은 지워짐. (중요 데이터 방지를 위해, 기존 graph는 복사 됨.) 
![5_regist_ConnectionType_to_NodeGraph.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/5_regist_ConnectionType_to_NodeGraph.gif)


5. SimpleGraphBuilder 실행: 
    "Window" -> "UI Toolkit" -> "SimpleGraphBuilder"를 통해 SimpleGraphBuilder 접근하기.
![6_Open_SimpleGraphBuilder.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/6_Open_SimpleGraphBuilder.gif)


6. NodeGraph 선택: 
    SimpleGraphBuilder에서 만들어둔 NodeGraph선택.
7. Root Node 만들기: 
    왼클릭을 통해 "Create Root Node" 를 선택 (모든 노드는 SimpleGraphBuilder상에서만 지워짐).
8. 추가 Nodes 만들기: 
    왼클릭을 통해 "Create Node".
9. Node 연결하기: 
    각 노드의 입출력 포트들에서부터 드래그 앤 드롭으로 연결 할 수 있음
![7_Make_Node.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/7_Make_Node.gif)


# 제공되는 사용할 만한 기능이 있나요?
1. SimpleGraphBuilder상의 Node클릭시, 해당 노드가 프로젝트에서 하이라이트 됩니다.
![Func1_Highlight.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Func1_Highlight.gif)

2. GraphBuild의 각 노드 안에 있는 objectField에 Object를 넣을 경우 각 노드의 ConnectedObj에 자동으로 등록됩니다.  
![Func2_Auto_Connect.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Func2_Auto_Connect.gif)

3. UnDo & ReDo 지원하고...(ctrl + z or ctrl + y)
![Func3_Un%26ReDo.gif](https://github.com/lumenCat92/SimpleGraphBuilder/blob/main/Image/Func3_Un%26ReDo.gif)

4. 타겟 오브젝트로부터 root Node까지의 2가지 길찾기를 지원합니다. 

```csharp
NodeGraph.FindPathWithMixConnectionType(Node currentNode, List<string> findConnectionType, ref List<List<Node>> pathNodes, ref List<Node> path, ref bool isPathDone, bool shouldDoneWithFirstFinding);
```
= findConnectionType에 지정된 모든 연결을 혼합한 경로를 찾음.

```csharp
NodeGraph.FindPathWithEachConnectionType(Node currentNode, List<string> findConnectionType);
```
= findConnectionType에 지정된 각각의 연결들에 대한 경로를 찾음.
</details>