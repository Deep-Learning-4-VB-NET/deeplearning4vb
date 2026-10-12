Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports org.deeplearning4j.util
Imports org.nd4j.adapters
Imports AsyncMultiDataSetIterator = org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports org.deeplearning4j.nn.api
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports RecurrentLayer = org.deeplearning4j.nn.api.layers.RecurrentLayer
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraphUtil = org.deeplearning4j.nn.graph.util.ComputationGraphUtil
Imports GraphIndices = org.deeplearning4j.nn.graph.util.GraphIndices
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports VertexIndices = org.deeplearning4j.nn.graph.vertex.VertexIndices
Imports FrozenVertex = org.deeplearning4j.nn.graph.vertex.impl.FrozenVertex
Imports InputVertex = org.deeplearning4j.nn.graph.vertex.impl.InputVertex
Imports LayerVertex = org.deeplearning4j.nn.graph.vertex.impl.LayerVertex
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.layers.FrozenLayerWithBackprop
Imports BidirectionalLayer = org.deeplearning4j.nn.layers.recurrent.BidirectionalLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Solver = org.deeplearning4j.optimize.Solver
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetUtil = org.nd4j.linalg.dataset.api.DataSetUtil
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Heartbeat = org.nd4j.linalg.heartbeat.Heartbeat
Imports Environment = org.nd4j.linalg.heartbeat.reports.Environment
Imports [Event] = org.nd4j.linalg.heartbeat.reports.Event
Imports Task = org.nd4j.linalg.heartbeat.reports.Task
Imports EnvironmentUtils = org.nd4j.linalg.heartbeat.utils.EnvironmentUtils
Imports TaskUtils = org.nd4j.linalg.heartbeat.utils.TaskUtils
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports DummyWorkspace = org.nd4j.linalg.api.memory.abstracts.DummyWorkspace
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports ND4JWorkspaceException = org.nd4j.linalg.workspace.ND4JWorkspaceException
Imports WorkspaceUtils = org.nd4j.linalg.workspace.WorkspaceUtils
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger
Imports WorkspacesCloseable = org.nd4j.linalg.workspace.WorkspacesCloseable

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.nn.graph


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ComputationGraph implements Serializable, Model, NeuralNetwork
	<Serializable>
	Public Class ComputationGraph
		Implements Model, NeuralNetwork

'JAVA TO VB CONVERTER NOTE: The field configuration was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend configuration_Conflict As ComputationGraphConfiguration
		Protected Friend initCalled As Boolean = False
		<NonSerialized>
		Protected Friend solver As Solver 'Used to call optimizers during backprop
		Protected Friend flattenedParams As INDArray 'Params for all layers are a view/subset of this array
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient org.nd4j.linalg.api.ndarray.INDArray flattenedGradients;
		<NonSerialized>
		Protected Friend flattenedGradients As INDArray 'Gradients for all layers are a view/subset of this array
'JAVA TO VB CONVERTER NOTE: The field gradient was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradient_Conflict As Gradient
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend score_Conflict As Double
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private boolean initDone = false;
		Private initDone As Boolean = False
		Protected Friend clearTbpttState As Boolean = True 'Mainly for unit testing (should be enabled otherwise)
		'Workspaces for CUDNN. Pass to LayerWorkspaceMgr for re-use in cudnn helpers
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient Map<String,org.bytedeco.javacpp.Pointer> helperWorkspaces = new HashMap<>();
		<NonSerialized>
		Protected Friend helperWorkspaces As IDictionary(Of String, Pointer) = New Dictionary(Of String, Pointer)()

		<NonSerialized>
		Private ReadOnly occupiedBy As New AtomicLong(-1)

		''' <summary>
		''' Workspace for working memory for a single layer: forward pass and backward pass
		''' Note that this is opened/closed once per op (activate/backpropGradient call)
		''' </summary>
		Protected Friend Const WS_LAYER_WORKING_MEM As String = "WS_LAYER_WORKING_MEM"
		''' <summary>
		''' Workspace for storing all layers' activations - used only to store activations (layer inputs) as part of backprop
		''' Not used for inference
		''' </summary>
		Protected Friend Const WS_ALL_LAYERS_ACT As String = "WS_ALL_LAYERS_ACT"
		''' <summary>
		''' Workspace for working memory in RNNs - opened and closed once per RNN time step
		''' </summary>
		Protected Friend Const WS_RNN_LOOP_WORKING_MEM As String = "WS_RNN_LOOP_WORKING_MEM"

		''' <summary>
		''' Workspace for output methods that use OutputAdapter
		''' </summary>
		Protected Friend Const WS_OUTPUT_MEM As String = "WS_OUTPUT_MEM"

		Protected Friend ReadOnly WS_LAYER_WORKING_MEM_CONFIG As WorkspaceConfiguration

		Protected Friend Shared ReadOnly WS_ALL_LAYERS_ACT_CONFIG As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.05).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

		Protected Friend ReadOnly WS_LAYER_ACT_X_CONFIG As WorkspaceConfiguration

		Protected Friend Shared ReadOnly WS_RNN_LOOP_WORKING_MEM_CONFIG As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.05).policyReset(ResetPolicy.BLOCK_LEFT).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).build()


'JAVA TO VB CONVERTER NOTE: The field lastEtlTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend lastEtlTime_Conflict As New ThreadLocal(Of Long)()

		''' <summary>
		''' All GraphVertex objects in the network.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field vertices was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend vertices_Conflict() As GraphVertex
		''' <summary>
		''' Map of vertices by name
		''' </summary>
		Protected Friend verticesMap As IDictionary(Of String, GraphVertex)
		''' <summary>
		''' Indexes of graph vertices, in topological order. The topological order defines the order in which forward pass
		''' (and hence also backward pass, which is the opposite to this) is conducted in the network.
		''' </summary>
		Protected Friend topologicalOrder() As Integer
		''' <summary>
		''' Topological sort and vertex index/name + name/index mapping
		''' </summary>
		Protected Friend graphIndices As GraphIndices

		''' <summary>
		''' A list of layers. Each of these layers is present in a GraphVertex, but are here for easy reference.
		''' This array also defines the order in which the getLayer(int) method returns layers.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field layers was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend layers_Conflict() As Layer

		''' <summary>
		''' The number of input arrays to the network. Many networks only have 1 input; however, a ComputationGraph may
		''' have an arbitrary number (>=1) separate input arrays
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field numInputArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numInputArrays_Conflict As Integer
		''' <summary>
		''' The number of output arrays to the network. Many networks only have 1 output; however, a ComputationGraph may
		''' have an arbitrary number (>=1) separate output arrays
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field numOutputArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numOutputArrays_Conflict As Integer

		'Current inputs, labels, input mask arrays and label mask arrays
'JAVA TO VB CONVERTER NOTE: The field inputs was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private inputs_Conflict() As INDArray
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private labels_Conflict() As INDArray
'JAVA TO VB CONVERTER NOTE: The field inputMaskArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private inputMaskArrays_Conflict() As INDArray
'JAVA TO VB CONVERTER NOTE: The field labelMaskArrays was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private labelMaskArrays_Conflict() As INDArray

		<NonSerialized>
		Private outputLayerIdxs() As Integer

		Private defaultConfiguration As NeuralNetConfiguration
		Private trainingListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()


		Public Sub New(ByVal configuration As ComputationGraphConfiguration)
			Me.configuration_Conflict = configuration
			Me.numInputArrays_Conflict = configuration.getNetworkInputs().size()
			Me.numOutputArrays_Conflict = configuration.getNetworkOutputs().size()
			Me.inputs_Conflict = New INDArray(numInputArrays_Conflict - 1){}
			Me.labels_Conflict = New INDArray(numOutputArrays_Conflict - 1){}
			Me.defaultConfiguration = configuration.getDefaultConfiguration()

			'Working memory: should learn over course of: (a) full forward pass, and (b) full backward pass
			'Working memory should be opened once per vertex, for each of forward and backward passes
			Dim numWorkingMem As Integer = 2 * configuration.getVertices().size()
			WS_LAYER_WORKING_MEM_CONFIG = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(numWorkingMem).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

			'Activations memory: opened once per layer - for every second layer (preprocessors are within the loop).
			'Technically we could set learning to numLayers / 2, but will set to numLayers for simplicity, and also to
			' account for a backward pass
			WS_LAYER_ACT_X_CONFIG = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(configuration.getVertices().size()).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()
		End Sub

		''' <summary>
		''' This method allows to set ETL field time, useful for performance tracking
		''' </summary>
		''' <param name="time"> </param>
		Public Overridable Property LastEtlTime As Long
			Set(ByVal time As Long)
				lastEtlTime_Conflict.set(time)
			End Set
			Get
				Dim time As Long? = lastEtlTime_Conflict.get()
				Return If(time Is Nothing, 0L, time)
			End Get
		End Property


		''' <summary>
		''' This method sets specified CacheMode for all layers within network
		''' </summary>
		''' <param name="mode"> </param>
		Public Overridable WriteOnly Property CacheMode As CacheMode
			Set(ByVal mode As CacheMode)
				If mode = Nothing Then
					mode = CacheMode.NONE
				End If
    
				For Each layer As Layer In layers_Conflict
					layer.CacheMode = mode
				Next layer
			End Set
		End Property

		''' <summary>
		''' This method returns configuration of this ComputationGraph
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Configuration As ComputationGraphConfiguration
			Get
				Return configuration_Conflict
			End Get
		End Property

		''' <summary>
		''' Returns the number of layers in the ComputationGraph
		''' </summary>
		Public Overridable ReadOnly Property NumLayers As Integer
			Get
				Return (If(layers_Conflict IsNot Nothing, layers_Conflict.Length, 0))
			End Get
		End Property

		''' <summary>
		''' Get the layer by the number of that layer, in range 0 to getNumLayers()-1
		''' NOTE: This is different from the internal GraphVertex index for the layer
		''' </summary>
		Public Overridable Function getLayer(ByVal idx As Integer) As Layer
			Return layers_Conflict(idx)
		End Function

		''' <summary>
		''' Get all layers in the ComputationGraph
		''' </summary>
		Public Overridable ReadOnly Property Layers As Layer()
			Get
				Return layers_Conflict
			End Get
		End Property

		''' <summary>
		''' Get a given layer by name.
		''' </summary>
		Public Overridable Function getLayer(ByVal name As String) As Layer
			Preconditions.checkState(verticesMap.ContainsKey(name), "Layer with name %s does not exist in the network", name)
			Return verticesMap(name).getLayer()
		End Function

		''' <summary>
		''' Returns an array of all GraphVertex objects.
		''' </summary>
		Public Overridable ReadOnly Property Vertices As GraphVertex()
			Get
				Return vertices_Conflict
			End Get
		End Property

		''' <summary>
		''' Return a given GraphVertex by name, or null if no vertex with that name exists
		''' </summary>
		Public Overridable Function getVertex(ByVal name As String) As GraphVertex
			Return verticesMap(name)
		End Function

		''' <summary>
		''' The number of inputs to this network
		''' </summary>
		Public Overridable ReadOnly Property NumInputArrays As Integer
			Get
				Return numInputArrays_Conflict
			End Get
		End Property

		''' <summary>
		''' The number of output (arrays) for this network
		''' </summary>
		Public Overridable ReadOnly Property NumOutputArrays As Integer
			Get
				Return numOutputArrays_Conflict
			End Get
		End Property

		''' <summary>
		''' Set the specified input for the ComputationGraph
		''' </summary>
		Public Overridable Sub setInput(ByVal inputNum As Integer, ByVal input As INDArray)
			If inputs_Conflict Is Nothing Then
				'May be null after clear()
				inputs_Conflict = New INDArray(numInputArrays_Conflict - 1){}
			End If
			inputs_Conflict(inputNum) = input
		End Sub

		''' <summary>
		''' Set all inputs for the ComputationGraph network
		''' </summary>
		Public Overridable Property Inputs As INDArray()
			Set(ByVal inputs() As INDArray)
				If inputs IsNot Nothing AndAlso inputs.Length <> Me.numInputArrays_Conflict Then
					Throw New System.ArgumentException("Invalid input array: network has " & numInputArrays_Conflict & " inputs, but array is of length " & inputs.Length)
				End If
				Me.inputs_Conflict = inputs
			End Set
			Get
				Return inputs_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the previously set input for the ComputationGraph
		''' </summary>
		Public Overridable Function getInput(ByVal inputNum As Integer) As INDArray
			If inputs_Conflict Is Nothing Then
				Return Nothing
			End If
			Return inputs_Conflict(inputNum)
		End Function


		''' <summary>
		''' Get the previously set feature/input mask arrays for the ComputationGraph
		''' </summary>
		Public Overridable ReadOnly Property InputMaskArrays As INDArray()
			Get
				Return inputMaskArrays_Conflict
			End Get
		End Property

		''' <summary>
		''' Get the previously set label/output mask arrays for the ComputationGraph
		''' </summary>
		Public Overridable ReadOnly Property LabelMaskArrays As INDArray()
			Get
				Return labelMaskArrays_Conflict
			End Get
		End Property

		''' <summary>
		''' Set the specified label for the ComputationGraph
		''' </summary>
		Public Overridable Sub setLabel(ByVal labelNum As Integer, ByVal label As INDArray)
			labels_Conflict(labelNum) = label
		End Sub

		''' <summary>
		''' Set all labels for the ComputationGraph network
		''' </summary>
		Public Overridable WriteOnly Property Labels As INDArray()
			Set(ByVal labels() As INDArray)
				If labels IsNot Nothing AndAlso labels.Length <> Me.numOutputArrays_Conflict Then
					Throw New System.ArgumentException("Invalid output array: network has " & numOutputArrays_Conflict & " outputs, but array is of length " & labels.Length)
				End If
				Me.labels_Conflict = labels
			End Set
		End Property

		''' <summary>
		''' This method allows you to specificy GradientsAccumulator instance to be used with this model
		''' <para>
		''' PLEASE NOTE: Do not use this method unless you understand how to use GradientsAccumulator & updates sharing.
		''' PLEASE NOTE: Do not use this method on standalone model
		''' 
		''' </para>
		''' </summary>
		''' <param name="accumulator"> </param>
		Public Overridable WriteOnly Property GradientsAccumulator As GradientsAccumulator
			Set(ByVal accumulator As GradientsAccumulator)
				If Not initCalled Then
					init()
				End If
    
				solver.Optimizer.GradientsAccumulator = accumulator
			End Set
		End Property

		''' <summary>
		''' Initialize the ComputationGraph network
		''' </summary>
		Public Overridable Sub init() Implements Model.init, NeuralNetwork.init
			init(Nothing, False)
		End Sub

		''' <summary>
		''' Initialize the ComputationGraph, optionally with an existing parameters array.
		''' If an existing parameters array is specified, it will be used (and the values will not be modified) in the network;
		''' if no parameters array is specified, parameters will be initialized randomly according to the network configuration.
		''' </summary>
		''' <param name="parameters">           Network parameter. May be null. If null: randomly initialize. </param>
		''' <param name="cloneParametersArray"> Whether the parameter array (if any) should be cloned, or used directly </param>
		Public Overridable Sub init(ByVal parameters As INDArray, ByVal cloneParametersArray As Boolean)
			If initCalled Then
				Return
			End If

			Dim netDtype As DataType = Configuration.getDataType()
			If parameters IsNot Nothing AndAlso parameters.dataType() <> netDtype Then
				Preconditions.checkState(parameters.rank() = 2 AndAlso parameters.size(0) = 1, "Invalid parameters array: should be rank 2 with shape [1,numParams]. Got %ndShape", parameters)
				If cloneParametersArray Then
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
						parameters = parameters.castTo(netDtype)
					End Using
				Else
					Throw New System.InvalidOperationException("Error initializing network: Network datatype is set to " & netDtype & " but provided array has datatype " & parameters.dataType() & " with cloneParametersArray argument" & " set to false. Cannot initialize net with specified datatype array if that array does not match network datatype")
				End If
			End If

			If configuration_Conflict.getTrainingWorkspaceMode() Is Nothing Then
				configuration_Conflict.setTrainingWorkspaceMode(WorkspaceMode.NONE)
			End If

			If configuration_Conflict.getInferenceWorkspaceMode() Is Nothing Then
				configuration_Conflict.setInferenceWorkspaceMode(WorkspaceMode.NONE)
			End If

			If configuration_Conflict.getCacheMode() Is Nothing Then
				configuration_Conflict.setCacheMode(CacheMode.NONE)
			End If

			OneTimeLogger.info(log, "Starting ComputationGraph with WorkspaceModes set to [training: {}; inference: {}], cacheMode set to [{}]", configuration_Conflict.getTrainingWorkspaceMode(), configuration_Conflict.getInferenceWorkspaceMode(), configuration_Conflict.getCacheMode())

			'First: build topological ordering, based on configuration. Used for forward pass, backprop and order of parameters/gradients
			Dim indices As GraphIndices = calculateIndices()
			topologicalOrder = indices.getTopologicalSortOrder()

			'Initialization: create the GraphVertex objects, based on configuration structure
			Dim configVertexMap As IDictionary(Of String, org.deeplearning4j.nn.conf.graph.GraphVertex) = configuration_Conflict.getVertices()

			'Names of all of the (data) inputs to the ComputationGraph
			Dim networkInputNames As IList(Of String) = configuration_Conflict.getNetworkInputs()

			'Inputs for each layer and GraphNode:
			Dim vertexInputs As IDictionary(Of String, IList(Of String)) = configuration_Conflict.getVertexInputs()
			Me.vertices_Conflict = New GraphVertex((networkInputNames.Count + configuration_Conflict.getVertices().size()) - 1){}

			'All names: inputs, layers and graph nodes (index to name map)
			Dim allNamesReverse As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

			'Create network input vertices:
			Dim vertexNumber As Integer = 0
			For Each name As String In networkInputNames
				Dim gv As GraphVertex = New InputVertex(Me, name, vertexNumber, Nothing, netDtype) 'Output vertices: set later
				allNamesReverse(name) = vertexNumber
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: vertices[vertexNumber++] = gv;
				vertices_Conflict(vertexNumber) = gv
					vertexNumber += 1
			Next name

			'Go through layers, and work out total number of parameters. Then allocate full parameters array
			Dim numParams As Long = 0
			Dim numParamsForVertex(topologicalOrder.Length - 1) As Long
			Dim i As Integer = 0
			Do While i < configuration_Conflict.getNetworkInputs().size()
				numParamsForVertex(i) = 0 'No parameters for input vertices
				i += 1
			Loop
			Do While i < topologicalOrder.Length
				Dim name As String = indices.getIdxToName().get(i)
				Dim n As org.deeplearning4j.nn.conf.graph.GraphVertex = configVertexMap(name)
				n.DataType = netDtype
				numParamsForVertex(i) = n.numParams(True)
				If numParamsForVertex(i) < 0 Then
					Throw New DL4JInvalidConfigException("Layer " & name & " had parameters < 0 " & numParamsForVertex(i))
				End If
				numParams += numParamsForVertex(i)
				i += 1
			Loop

			Dim initializeParams As Boolean
			If parameters IsNot Nothing Then
				If Not parameters.RowVectorOrScalar Then
					Throw New System.ArgumentException("Invalid parameters: should be a row vector")
				End If
				If parameters.length() <> numParams Then
					Throw New System.ArgumentException("Invalid parameters: expected length " & numParams & ", got length " & parameters.length())
				End If

				If cloneParametersArray Then
					flattenedParams = parameters.dup()
				Else
					flattenedParams = parameters
				End If

				initializeParams = False
			ElseIf numParams > 0 Then
				flattenedParams = Nd4j.create(netDtype, 1, numParams)
				initializeParams = True
			Else
				flattenedParams = Nothing
				initializeParams = False
			End If

			'Set RNG seed, for repeatability between initializations when set
			If initializeParams Then
				Nd4j.Random.setSeed(conf().getSeed())
			End If

			'Given the topological ordering: work out the subset of the parameters array used for each layer
			' Then extract out for use when initializing the Layers
			Dim paramsViewForVertex(topologicalOrder.Length - 1) As INDArray
			Dim paramOffsetSoFar As Long = 0
			i = 0
			For Each vertexIdx As Integer In topologicalOrder
				Dim nParamsThisVertex As Long = numParamsForVertex(vertexIdx)
				If nParamsThisVertex <> 0 Then
					paramsViewForVertex(vertexIdx) = flattenedParams.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramOffsetSoFar, paramOffsetSoFar + nParamsThisVertex))
				End If
				i += 1
				paramOffsetSoFar += nParamsThisVertex
			Next vertexIdx


			Dim numLayers As Integer = 0
			Dim tempLayerList As IList(Of Layer) = New List(Of Layer)()
			defaultConfiguration.clearVariables()
			Dim variables As IList(Of String) = defaultConfiguration.variables(False)
			i = configuration_Conflict.getNetworkInputs().size()
			Do While i<topologicalOrder.Length
				Dim name As String = indices.getIdxToName().get(i)
				Dim n As org.deeplearning4j.nn.conf.graph.GraphVertex = configVertexMap(name)

				Dim gv As GraphVertex = n.instantiate(Me, name, vertexNumber, paramsViewForVertex(vertexNumber), initializeParams, netDtype)

				If gv Is Nothing Then
					Throw New System.InvalidOperationException("Encountered null layer/vertex during initialization for layer """ & name & """: " & n.GetType().Name & " initialization returned null layer/vertex?")
				End If

				If gv.hasLayer() Then
					numLayers += 1
					Dim l As Layer = gv.Layer
					tempLayerList.Add(l)
					Dim layerVariables As IList(Of String) = l.conf().variables()
					If layerVariables IsNot Nothing Then
						For Each s As String In layerVariables
							variables.Add(gv.VertexName & "_" & s)
						Next s
					End If
				End If

				allNamesReverse(name) = vertexNumber
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: vertices[vertexNumber++] = gv;
				vertices_Conflict(vertexNumber) = gv
					vertexNumber += 1
				i += 1
			Loop
			layers_Conflict = CType(tempLayerList, List(Of Layer)).ToArray()

			'Create the lookup table, so we can find vertices easily by name
			verticesMap = New Dictionary(Of String, GraphVertex)()
			For Each gv As GraphVertex In vertices_Conflict
				verticesMap(gv.VertexName) = gv
			Next gv

			'Now: do another pass to set the input and output indices, for each vertex
			' These indices are used during forward and backward passes
			'To get output indices: need to essentially build the graph in reverse...
			Dim verticesOutputTo As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))() 'Key: vertex. Values: vertices that this node is an input for
			For Each gv As GraphVertex In vertices_Conflict
				Dim vertexName As String = gv.VertexName
				Dim vertexInputNames As IList(Of String)
				vertexInputNames = vertexInputs(vertexName)

				If vertexInputNames Is Nothing Then
					Continue For
				End If

				'Build reverse network structure:
				For Each s As String In vertexInputNames
					Dim list As IList(Of String) = verticesOutputTo(s)
					If list Is Nothing Then
						list = New List(Of String)()
						verticesOutputTo(s) = list
					End If
					list.Add(vertexName) 'Edge: s -> vertexName
				Next s
			Next gv


			For Each gv As GraphVertex In vertices_Conflict
				Dim vertexName As String = gv.VertexName
				Dim vertexIndex As Integer = gv.VertexIndex
				Dim vertexInputNames As IList(Of String)
				vertexInputNames = vertexInputs(vertexName)

				If vertexInputNames Is Nothing Then
					Continue For
				End If

				Dim inputIndices(vertexInputNames.Count - 1) As VertexIndices
				For j As Integer = 0 To vertexInputNames.Count - 1
					Dim inName As String = vertexInputNames(j)
					Dim inputVertexIndex As Integer = allNamesReverse(inName)

					'Here: we have x -> gv connection
					'gv might have multiple inputs, not just x
					'Need to know which input x is
					Dim inputNumber As Integer = vertexInputs(vertexName).IndexOf(inName)

					If inputNumber = -1 Then
						Throw New System.InvalidOperationException("Could not find vertex " & vertexIndex & " in the list of inputs " & "for vertex " & gv.VertexName & "; error in graph structure?")
					End If

					inputIndices(j) = New VertexIndices(inputVertexIndex, inputNumber)
				Next j

				gv.InputVertices = inputIndices
			Next gv

			'Handle the outputs for this vertex
			For Each gv As GraphVertex In vertices_Conflict
				Dim vertexName As String = gv.VertexName

				Dim thisVertexOutputsTo As IList(Of String) = verticesOutputTo(vertexName)

				If thisVertexOutputsTo Is Nothing OrElse thisVertexOutputsTo.Count = 0 Then
					Continue For 'Output vertex
				End If
				Dim outputIndices(thisVertexOutputsTo.Count - 1) As VertexIndices
				Dim j As Integer = 0
				For Each s As String In New HashSet(Of )(thisVertexOutputsTo)
					'First, we have gv -> s
					'Which input in s does gv connect to? s may in general have multiple inputs...
					Dim nextVertexInputNames As IList(Of String) = vertexInputs(s)

					For k As Integer = 0 To nextVertexInputNames.Count - 1
						If vertexName.Equals(nextVertexInputNames(k)) Then
							Dim outputVertexIndex As Integer = allNamesReverse(s)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: outputIndices[j++] = new org.deeplearning4j.nn.graph.vertex.VertexIndices(outputVertexIndex, k);
							outputIndices(j) = New VertexIndices(outputVertexIndex, k)
								j += 1
						End If
					Next k
				Next s
				gv.OutputVertices = outputIndices
			Next gv

			'Mark any output vertices as outputs:
			For Each s As String In configuration_Conflict.getNetworkOutputs()
				Dim gv As GraphVertex = verticesMap(s)
				gv.OutputVertex = True
			Next s

			' now we init solver & optimizer
			If solver Is Nothing Then
				Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
					solver.initOptimizer()
				End Using
			End If

			'Mark which layers can safely modify their input in-place. This is a performance optimization for
			' dropout and similar operations.
			' Safe when the input is: (a) it's not a graph input, and (b) isn't shared by any other layers/vertices

			Dim seenAsInputTo As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))()
			For Each entry As KeyValuePair(Of String, IList(Of String)) In configuration_Conflict.getVertexInputs().entrySet()
				For Each s As String In entry.Value
					If Not seenAsInputTo.ContainsKey(s) Then
						seenAsInputTo(s) = New List(Of String)()
					End If
					Dim seen As IList(Of String) = seenAsInputTo(s)
					seen.Add(entry.Key)
				Next s
			Next entry

			For Each l As Layer In layers_Conflict
				Dim layerName As String = l.conf().getLayer().getLayerName()
				Dim inputs As IList(Of String) = configuration_Conflict.getVertexInputs().get(layerName)
				Dim [in] As String = inputs(0) 'For now: layers should have exactly 1 input

				If configuration_Conflict.getNetworkInputs().contains([in]) Then
					'TODO When is it safe to NOT allow input modifucation? It's not always safe...
					' For example dropout + iterating over List<MultiDataSet> that is used for multiple epochs...
					Continue For
				End If

				Dim seen As IList(Of String) = seenAsInputTo([in])
				If seen.Count = 1 Then
					l.allowInputModification(True)
				Else
					'For the count > 1 case, we can work out if it's the last one in the topological order... at which point,
					' it should be safe to use
					Dim thisIdx As Integer = indices.getNameToIdx().get(layerName)
					Dim thisTopoPos As Integer = ArrayUtils.IndexOf(indices.getTopologicalSortOrder(), thisIdx)
					Dim maxTopoPosition As Integer = -1
					For Each s As String In seen
						Dim idx As Integer = indices.getNameToIdx().get(s)
						Dim topoPos As Integer = ArrayUtils.IndexOf(indices.getTopologicalSortOrder(), idx)
						maxTopoPosition = Math.Max(maxTopoPosition, topoPos)
					Next s

					If thisTopoPos = maxTopoPosition Then
						'Last one in the topo sort... all other layers have already consumed this input by the time this layer's
						' forward pass is done
						l.allowInputModification(True)
					End If 'Otherwise: keep default of false
				End If
			Next l

			synchronizeIterEpochCounts()
			initCalled = True
		End Sub

		''' <summary>
		''' This method: initializes the flattened gradients array (used in backprop) and sets the appropriate subset in all layers.
		''' As a general rule, this shouldn't ever need to be called manually when doing training via fit(DataSet), fit(DataSetIterator)
		''' or fit(MultiDataSet) methods
		''' </summary>
		Public Overridable Sub initGradientsView()
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				If Not initCalled Then
					init()
				End If

				Dim indices As GraphIndices = calculateIndices()

				'Go through layers, and work out total number of parameters. Then allocate full parameters array
				Dim numParams As Long = 0
				Dim numParamsForVertex(topologicalOrder.Length - 1) As Long
				Dim i As Integer = 0
				Do While i < configuration_Conflict.getNetworkInputs().size()
					numParamsForVertex(i) = 0 'No parameters for input vertices
					i += 1
				Loop
				Dim configVertexMap As IDictionary(Of String, org.deeplearning4j.nn.conf.graph.GraphVertex) = configuration_Conflict.getVertices()
				Do While i < topologicalOrder.Length
					Dim name As String = indices.getIdxToName().get(i)
					Dim n As org.deeplearning4j.nn.conf.graph.GraphVertex = configVertexMap(name)
					numParamsForVertex(i) = n.numParams(True)
					numParams += numParamsForVertex(i)
					i += 1
				Loop

				If numParams > 0 Then
					flattenedGradients = Nd4j.create(flattenedParams.dataType(), 1, numParams)
				End If

				'Given the topological ordering: work out the subset of the gradient array used for each layer, and set it
				Dim paramOffsetSoFar As Long = 0
				i = 0
				For Each vertexIdx As Integer In topologicalOrder
					Dim nParamsThisVertex As Long = numParamsForVertex(vertexIdx)
					If nParamsThisVertex <> 0 Then
						Dim gradientView As INDArray = flattenedGradients.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramOffsetSoFar, paramOffsetSoFar + nParamsThisVertex))
						vertices_Conflict(vertexIdx).BackpropGradientsViewArray = gradientView
					End If
					i += 1
					paramOffsetSoFar += nParamsThisVertex
				Next vertexIdx
			End Using
		End Sub

		Protected Friend Overridable ReadOnly Property OutputLayerIndices As Integer()
			Get
				If outputLayerIdxs Is Nothing Then
					outputLayerIdxs = New Integer(numOutputArrays_Conflict - 1){}
					Dim i As Integer = 0
					For Each s As String In configuration_Conflict.getNetworkOutputs()
	'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
	'ORIGINAL LINE: outputLayerIdxs[i++] = verticesMap.get(s).getVertexIndex();
						outputLayerIdxs(i) = verticesMap(s).getVertexIndex()
							i += 1
					Next s
				End If
				Return outputLayerIdxs
			End Get
		End Property

		''' <summary>
		''' Perform layerwise pretraining for one epoch - see <seealso cref="pretrain(DataSetIterator, Integer)"/>
		''' </summary>
		Public Overridable Sub pretrain(ByVal iter As DataSetIterator)
			pretrain(iter, 1)
		End Sub

		''' <summary>
		''' Pretrain network with a single input and single output. DataSetIterators can only be used if the number of input
		''' arrays for the ComputationGraph is 1.<br>
		''' This method performs layerwise pretraining on all pre-trainable layers in the network (VAEs, Autoencoders, etc), for the specified
		''' number of epochs each. For example, if numEpochs=3, then layer 0 will be fit for 3 epochs, followed by layer 1
		''' for 3 epochs, and so on.<br>
		''' For networks with more than one input use <seealso cref="pretrain(MultiDataSetIterator)"/>
		''' </summary>
		Public Overridable Sub pretrain(ByVal iter As DataSetIterator, ByVal numEpochs As Integer)
			If numInputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot train ComputationGraph network with  multiple inputs using a DataSetIterator")
			End If

			pretrain(ComputationGraphUtil.toMultiDataSetIterator(iter), numEpochs)
		End Sub

		''' <summary>
		''' Pretrain network with multiple inputs and/or outputs
		''' </summary>
		Public Overridable Sub pretrain(ByVal iter As MultiDataSetIterator)
			pretrain(iter, 1)
		End Sub

		''' <summary>
		''' Pretrain network with multiple inputs and/or outputs<br>
		''' This method performs layerwise pretraining on all pre-trainable layers in the network (VAEs, Autoencoders, etc), for the specified
		''' number of epochs each. For example, if numEpochs=3, then layer 0 will be fit for 3 epochs, followed by layer 1
		''' for 3 epochs, and so on.<br>
		''' Non-pretrainable layers are ignored
		''' </summary>
		''' <param name="iter">      Training data </param>
		''' <param name="numEpochs"> Number of epochs to fit each layer with </param>
		''' <seealso cref= #pretrainLayer(String, MultiDataSetIterator) </seealso>
		Public Overridable Sub pretrain(ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer)
			Try
				pretrainHelper(iter, numEpochs)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Sub

		Private Sub pretrainHelper(ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer)
			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If

			'Assume here that all layers are pretrainable layers
			For i As Integer = 0 To topologicalOrder.Length - 1
				If Not vertices_Conflict(i).hasLayer() Then
					Continue For
				End If
				If TypeOf vertices_Conflict(i).Layer Is IOutputLayer Then
					Continue For 'Don't pretrain output layer
				End If
				If Not vertices_Conflict(i).Layer.PretrainLayer Then
					Continue For 'Skip layers that aren't pretrainable
				End If

				pretrainLayerHelper(vertices_Conflict(i).VertexName, iter, numEpochs)
			Next i
		End Sub

		''' <summary>
		''' Pretrain a specified layer with the given DataSetIterator
		''' </summary>
		''' <param name="layerName">       Layer name </param>
		''' <param name="dataSetIterator"> Data </param>
		Public Overridable Sub pretrainLayer(ByVal layerName As String, ByVal dataSetIterator As DataSetIterator)
			If numInputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot train ComputationGraph network with  multiple inputs using a DataSetIterator")
			End If

			pretrainLayer(layerName, ComputationGraphUtil.toMultiDataSetIterator(dataSetIterator))
		End Sub

		''' <summary>
		''' Pretrain a specified layer with the given MultiDataSetIterator
		''' </summary>
		''' <param name="layerName"> Layer name </param>
		''' <param name="iter">      Training data </param>
		Public Overridable Sub pretrainLayer(ByVal layerName As String, ByVal iter As MultiDataSetIterator)
			Try
				pretrainLayerHelper(layerName, iter, 1)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Sub

		Private Sub pretrainLayerHelper(ByVal layerName As String, ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer)
			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If

			If Not verticesMap.ContainsKey(layerName) Then
				Throw New System.InvalidOperationException("Invalid vertex name: " & layerName & " - all vertex names: " & verticesMap.Keys)
			End If
			If Not verticesMap(layerName).hasLayer() Then
				'No op
				Return
			End If

			Dim toTrain As GraphVertex = verticesMap(layerName)
			Dim idx As Integer = toTrain.VertexIndex

			Dim workspaceMgr As LayerWorkspaceMgr
			If configuration_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.UPDATER_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).build()
			End If
			workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iter.hasNext() AndAlso iter.resetSupported() Then
				iter.reset()
			End If

			Dim withAsync As MultiDataSetIterator = If(iter.asyncSupported(), New AsyncMultiDataSetIterator(iter), iter)

			Do While withAsync.MoveNext()
				Dim mds As MultiDataSet = withAsync.Current
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
					'FF - note should be using TEST mode here for the layers that feed into the specified layer
					Dim activations As IDictionary(Of String, INDArray) = ffToLayerActivationsInWS(False, idx, New Integer(){idx}, FwdPassType.STANDARD, False, mds.Features, mds.FeaturesMaskArrays, mds.LabelsMaskArrays, True)

					'Get input to the current layer
					Dim inputsToLayer() As VertexIndices = toTrain.InputVertices
					For Each vi As VertexIndices In inputsToLayer
						Dim inName As String = vertices_Conflict(vi.VertexIndex).VertexName
						Dim act As INDArray = activations(inName)
						toTrain.setInput(vi.VertexEdgeNumber, act, workspaceMgr)
					Next vi

					Dim layer As Layer = toTrain.Layer
					layer.fit(layer.input(), workspaceMgr)
				End Using
			Loop
		End Sub

		''' <summary>
		''' Fit the ComputationGraph using a DataSet.
		''' Note that this method can only be used with ComputationGraphs with 1 input and 1 output.
		''' For networks with more than one input or output, use <seealso cref="fit(MultiDataSetIterator)"/>
		''' </summary>
		Public Overridable Sub fit(ByVal dataSet As DataSet) Implements NeuralNetwork.fit
			If numInputArrays_Conflict <> 1 OrElse numOutputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot train ComputationGraph network with " & " multiple inputs or outputs using a DataSet")
			End If

			Dim hasMaskArrays As Boolean = dataSet.hasMaskArrays()
			If hasMaskArrays Then
				Dim fMask() As INDArray = (If(dataSet.FeaturesMaskArray IsNot Nothing, New INDArray(){dataSet.FeaturesMaskArray}, Nothing))
				Dim lMask() As INDArray = (If(dataSet.LabelsMaskArray IsNot Nothing, New INDArray(){dataSet.LabelsMaskArray}, Nothing))
				fit(New INDArray(){dataSet.Features}, New INDArray(){dataSet.Labels}, fMask, lMask)
			Else
				fit(New INDArray(){dataSet.Features}, New INDArray(){dataSet.Labels})
			End If

			If hasMaskArrays Then
				clearLayerMaskArrays()
			End If

			clearLayersStates()
		End Sub

		''' <summary>
		''' Perform minibatch training on all minibatches in the DataSetIterator, for the specified number of epochs.
		''' Equvalent to calling <seealso cref="fit(DataSetIterator)"/> numEpochs times in a loop
		''' </summary>
		''' <param name="iterator">  Training data (DataSetIterator). Iterator must support resetting </param>
		''' <param name="numEpochs"> Number of training epochs, >= 1 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void fit(@NonNull DataSetIterator iterator, int numEpochs)
		Public Overridable Sub fit(ByVal iterator As DataSetIterator, ByVal numEpochs As Integer)
			Preconditions.checkArgument(numEpochs > 0, "Number of epochs much be > 0. Got numEpochs = %s", numEpochs)
			Preconditions.checkArgument(numEpochs = 1 OrElse iterator.resetSupported(), "Cannot perform multiple epochs training using" & "iterator thas does not support resetting (iterator.resetSupported() returned false)")

			For i As Integer = 0 To numEpochs - 1
				fit(iterator)
			Next i
		End Sub

		''' <summary>
		''' Fit the ComputationGraph using a DataSetIterator.<br>
		''' Note that this method can only be used with ComputationGraphs with 1 input and 1 output<br>
		''' Method doesn't do layerwise  pretraining.<br>
		''' For pretraining use method pretrain.. <seealso cref="pretrain(DataSetIterator)"/><br> </summary>
		''' <param name="iterator"> Training data (DataSetIterator) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void fit(@NonNull DataSetIterator iterator)
		Public Overridable Sub fit(ByVal iterator As DataSetIterator)
			fit(New MultiDataSetIteratorAdapter(iterator))
		End Sub

		''' <summary>
		''' Fit the ComputationGraph using a MultiDataSet
		''' </summary>
		Public Overridable Sub fit(ByVal multiDataSet As MultiDataSet) Implements NeuralNetwork.fit
			fit(multiDataSet.Features, multiDataSet.Labels, multiDataSet.FeaturesMaskArrays, multiDataSet.LabelsMaskArrays)
			If multiDataSet.hasMaskArrays() Then
				clearLayerMaskArrays()
			End If
		End Sub

		''' <summary>
		''' Perform minibatch training on all minibatches in the MultiDataSetIterator, for the specified number of epochs.
		''' Equvalent to calling <seealso cref="fit(MultiDataSetIterator)"/> numEpochs times in a loop
		''' </summary>
		''' <param name="iterator">  Training data (DataSetIterator). Iterator must support resetting </param>
		''' <param name="numEpochs"> Number of training epochs, >= 1 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void fit(@NonNull MultiDataSetIterator iterator, int numEpochs)
		Public Overridable Sub fit(ByVal iterator As MultiDataSetIterator, ByVal numEpochs As Integer)
			Preconditions.checkArgument(numEpochs > 0, "Number of epochs much be > 0. Got numEpochs = %s", numEpochs)
			Preconditions.checkArgument(numEpochs = 1 OrElse iterator.resetSupported(), "Cannot perform multiple epochs training using" & "iterator thas does not support resetting (iterator.resetSupported() returned false)")

			For i As Integer = 0 To numEpochs - 1
				fit(iterator)
			Next i
		End Sub

		''' <summary>
		''' Fit the ComputationGraph using a MultiDataSetIterator
		''' Method doesn't do layerwise  pretraining.<br>
		''' For pretraining use method pretrain.. <seealso cref="pretrain(MultiDataSetIterator)"/><br> </summary>
		''' <param name="multi"> Training data (MultiDataSetIterator) </param>
		Public Overridable Sub fit(ByVal multi As MultiDataSetIterator) Implements NeuralNetwork.fit
			SyncLock Me
				If flattenedGradients Is Nothing Then
					initGradientsView()
				End If
        
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not multi.hasNext() AndAlso multi.resetSupported() Then
					multi.reset()
				End If
        
				For Each tl As TrainingListener In trainingListeners
					tl.onEpochStart(Me)
				Next tl
        
				Dim destructable As Boolean = False
        
				Dim multiDataSetIterator As MultiDataSetIterator
				If multi.asyncSupported() Then
					multiDataSetIterator = New AsyncMultiDataSetIterator(multi, Math.Max(Nd4j.AffinityManager.NumberOfDevices * 2, 2), True)
					destructable = True
				Else
					multiDataSetIterator = multi
				End If
        
				Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
				Do While multiDataSetIterator.MoveNext()
					Dim mds As MultiDataSet = multiDataSetIterator.Current
					Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
					lastEtlTime_Conflict.set((time2 - time1))
        
					fit(mds.Features,mds.Labels, mds.FeaturesMaskArrays, mds.LabelsMaskArrays)
					time1 = DateTimeHelper.CurrentUnixTimeMillis()
				Loop
        
				If destructable Then
					DirectCast(multiDataSetIterator, AsyncMultiDataSetIterator).shutdown()
				End If
        
				For Each tl As TrainingListener In trainingListeners
					tl.onEpochEnd(Me)
				Next tl
        
				incrementEpochCount()
			End SyncLock
		End Sub
		''' <summary>
		''' Fit the ComputationGraph given arrays of inputs and labels.
		''' </summary>
		''' <param name="inputs"> The network inptus </param>
		''' <param name="labels"> The labels </param>
		Public Overridable Sub fit(ByVal inputs() As INDArray, ByVal labels() As INDArray)
			fit(inputs, labels, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Fit the ComputationGraph using the specified inputs and labels (and mask arrays)
		''' </summary>
		''' <param name="inputs">            The network inputs (features) </param>
		''' <param name="labels">            The network labels </param>
		''' <param name="featureMaskArrays"> Mask arrays for inputs/features. Typically used for RNN training. May be null. </param>
		''' <param name="labelMaskArrays">   Mas arrays for the labels/outputs. Typically used for RNN training. May be null. </param>
		Public Overridable Sub fit(ByVal inputs() As INDArray, ByVal labels() As INDArray, ByVal featureMaskArrays() As INDArray, ByVal labelMaskArrays() As INDArray)
			Try
				fitHelper(inputs, labels, featureMaskArrays, labelMaskArrays)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Sub

		Private Sub fitHelper(ByVal inputs() As INDArray, ByVal labels() As INDArray, ByVal featureMaskArrays() As INDArray, ByVal labelMaskArrays() As INDArray)
			SyncLock Me
				If numParams() = 0 Then
					Return 'Edge case: net with no params: fitting is a no-op
				End If
        
				If flattenedGradients Is Nothing Then
					initGradientsView()
				End If
        
				Me.Inputs = inputs
				Me.Labels = labels
				setLayerMaskArrays(featureMaskArrays, labelMaskArrays)
				update(TaskUtils.buildTask(inputs, labels))
        
				Dim workspaceMgr As LayerWorkspaceMgr
				If configuration_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
					workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
				Else
					workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.UPDATER_WORKING_MEM, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).build()
				End If
				workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)
        
				If configuration_Conflict.getBackpropType() = BackpropType.TruncatedBPTT Then
					doTruncatedBPTT(inputs, labels, featureMaskArrays, labelMaskArrays, workspaceMgr)
				Else
					If solver Is Nothing Then
						Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
							solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
						End Using
					End If
        
					'TODO: cache workspace
					solver.optimize(workspaceMgr)
        
				End If
        
				If featureMaskArrays IsNot Nothing OrElse labelMaskArrays IsNot Nothing Then
					clearLayerMaskArrays()
				End If
        
				clearLayersStates()
				synchronizeIterEpochCounts()
			End SyncLock
		End Sub



		''' <summary>
		''' Calculate a topological sort order for the vertices in the graph.
		''' Note that this is used for
		''' (a) working out what order to do forward pass,
		''' (b) what order to do backprop (i.e., reverse of this)
		''' (c) order to flatten parameters (and gradients)
		''' <para>
		''' Specifically, gradients/params/forward pass are executed on vertex[topologicalSortOrder[i]], for i=0..nVertices-1
		''' </para>
		''' </summary>
		Public Overridable Function topologicalSortOrder() As Integer()
			Return calculateIndices().getTopologicalSortOrder()
		End Function

		''' <summary>
		''' Calculate the indices needed for the network:<br>
		''' (a) topological sort order<br>
		''' (b) Map: vertex index -> vertex name<br>
		''' (c) Map: vertex name -> vertex index<br>
		''' </summary>
		''' <returns> Calculated indices </returns>
		Public Overridable Function calculateIndices() As GraphIndices
			If graphIndices IsNot Nothing Then
				Return graphIndices
			End If


			'Get cached topological sort order from config, if present
			If configuration_Conflict.getTopologicalOrder() IsNot Nothing AndAlso configuration_Conflict.getTopologicalOrderStr() IsNot Nothing Then
				Dim t() As Integer = configuration_Conflict.getTopologicalOrder()
				Dim s As IList(Of String) = configuration_Conflict.getTopologicalOrderStr()
				Dim m1 As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
				Dim m2 As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
				For i As Integer = 0 To t.Length - 1
					m1(s(i)) = t(i)
					m2(t(i)) = s(i)
				Next i

				graphIndices = GraphIndices.builder().topologicalSortOrder(t).nameToIdx(m1).idxToName(m2).build()
				Return graphIndices
			End If


			'https://en.wikipedia.org/wiki/Topological_sorting#Kahn.27s_algorithm
			Dim nodeMap As IDictionary(Of String, org.deeplearning4j.nn.conf.graph.GraphVertex) = configuration_Conflict.getVertices()
			Dim networkInputNames As IList(Of String) = configuration_Conflict.getNetworkInputs()
			Dim numVertices As Integer = networkInputNames.Count + configuration_Conflict.getVertices().size()
			Dim [out](numVertices - 1) As Integer
			Dim outCounter As Integer = 0

			'First: represent the graph more usefully as a Map<Integer,Set<Integer>>, where map represents edges i -> j
			' key represents j, set is set of i (inputs) for vertices j
			Dim vertexNamesMap As IDictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
			Dim vertexNamesMap2 As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim i As Integer = 0
			For Each inputName As String In configuration_Conflict.getNetworkInputs()
				vertexNamesMap(i) = inputName
				vertexNamesMap2(inputName) = i
				i += 1
			Next inputName
			For Each entry As KeyValuePair(Of String, org.deeplearning4j.nn.conf.graph.GraphVertex) In nodeMap.SetOfKeyValuePairs()
				Dim name As String = entry.Key
				vertexNamesMap(i) = name
				vertexNamesMap2(name) = i
				i += 1
			Next entry

			Dim inputEdges As IDictionary(Of Integer, ISet(Of Integer)) = New Dictionary(Of Integer, ISet(Of Integer))() 'key: vertex. Values: vertices that the key vertex receives input from
			Dim outputEdges As IDictionary(Of Integer, ISet(Of Integer)) = New Dictionary(Of Integer, ISet(Of Integer))() 'key: vertex. Values: vertices that the key vertex outputs to

			For Each s As String In configuration_Conflict.getNetworkInputs()
				Dim idx As Integer = vertexNamesMap2(s)
				inputEdges(idx) = Nothing
			Next s

			For Each entry As KeyValuePair(Of String, org.deeplearning4j.nn.conf.graph.GraphVertex) In nodeMap.SetOfKeyValuePairs()
				Dim thisVertexName As String = entry.Key
				Dim idx As Integer = vertexNamesMap2(thisVertexName)
				Dim inputsToThisVertex As IList(Of String) = configuration_Conflict.getVertexInputs().get(thisVertexName)

				If inputsToThisVertex Is Nothing OrElse inputsToThisVertex.Count = 0 Then
					inputEdges(idx) = Nothing
					Continue For
				End If

				Dim inputSet As ISet(Of Integer) = New HashSet(Of Integer)()
				For Each s As String In inputsToThisVertex
					Dim inputIdx As Integer? = vertexNamesMap2(s)
					inputSet.Add(inputIdx)
					Dim outputSetForInputIdx As ISet(Of Integer) = outputEdges(inputIdx)
					If outputSetForInputIdx Is Nothing Then
						outputSetForInputIdx = New HashSet(Of Integer)()
						outputEdges(inputIdx) = outputSetForInputIdx
					End If
					outputSetForInputIdx.Add(idx) 'input vertex outputs to the current vertex
				Next s

				inputEdges(idx) = inputSet
			Next entry

			'Now: do topological sort
			'Set of all nodes with no incoming edges: (this would be: input vertices)
			Dim noIncomingEdges As New LinkedList(Of Integer)()
			For Each entry As KeyValuePair(Of Integer, ISet(Of Integer)) In inputEdges.SetOfKeyValuePairs()
				Dim inputsFrom As ISet(Of Integer) = entry.Value
				If inputsFrom Is Nothing OrElse inputsFrom.Count = 0 Then
					noIncomingEdges.AddLast(entry.Key)
				End If
			Next entry

			Do While noIncomingEdges.Count > 0
				Dim [next] As Integer = noIncomingEdges.RemoveFirst()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: out[outCounter++] = next;
				[out](outCounter) = [next] 'Add to sorted list
					outCounter += 1

				Dim vertexOutputsTo As ISet(Of Integer) = outputEdges([next])

				'Remove edges next -> vertexOuputsTo[...] from graph;
				If vertexOutputsTo IsNot Nothing Then
					For Each v As Integer? In vertexOutputsTo
						Dim set As ISet(Of Integer) = inputEdges(v)
						set.remove([next])
						If set.Count = 0 Then
							noIncomingEdges.AddLast(v) 'No remaining edges for vertex i -> add to list for processing
						End If
					Next v
				End If
			Loop

			'If any edges remain in the graph: graph has cycles:
			For Each entry As KeyValuePair(Of Integer, ISet(Of Integer)) In inputEdges.SetOfKeyValuePairs()
				Dim set As ISet(Of Integer) = entry.Value
				If set Is Nothing Then
					Continue For
				End If
				If set.Count > 0 Then
					Throw New System.InvalidOperationException("Invalid configuration: cycle detected in graph. Cannot calculate topological ordering with graph cycle (" & "cycle includes vertex """ & vertexNamesMap(entry.Key) & """)")
				End If
			Next entry

			'Store: the topological sort order in the configuraation... this is to ensure that when the config is
			' deserialized, it has exactly the same topological sort order on all platforms
			Dim s As IList(Of String) = New List(Of String)([out].Length)
			For Each idx As Integer In [out]
				s.Add(vertexNamesMap(idx))
			Next idx
			configuration_Conflict.setTopologicalOrder([out])
			configuration_Conflict.setTopologicalOrderStr(s)

			graphIndices = GraphIndices.builder().topologicalSortOrder([out]).nameToIdx(vertexNamesMap2).idxToName(vertexNamesMap).build()
			Return graphIndices
		End Function

		Public Overridable Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr) Implements Model.computeGradientAndScore
			computeGradientAndScore()
		End Sub

		Public Overridable Sub computeGradientAndScore()
			synchronizeIterEpochCounts()

			Dim workspaceMgr As LayerWorkspaceMgr
			If configuration_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.UPDATER_WORKING_MEM, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).build()
			End If
			workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)

			Dim tbptt As Boolean = configuration_Conflict.getBackpropType() = BackpropType.TruncatedBPTT
			Dim fwdType As FwdPassType = (If(tbptt, FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE, FwdPassType.STANDARD))
			synchronizeIterEpochCounts()

			'Calculate activations (which are stored in each layer, and used in backprop)
			Using wsAllActivations As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				Dim activations As IDictionary(Of String, INDArray) = ffToLayerActivationsInWS(True, -1, OutputLayerIndices, fwdType, tbptt, inputs_Conflict, inputMaskArrays_Conflict, labelMaskArrays_Conflict, False)
				If trainingListeners.Count > 0 Then
					Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						For Each tl As TrainingListener In trainingListeners
							tl.onForwardPass(Me, activations)
						Next tl
					End Using
				End If
				calcBackpropGradients(False,False)

				workspaceMgr.assertCurrentWorkspace(ArrayType.ACTIVATIONS, Nothing)

				'Score: sum of the scores for the various output layers...
				Dim r As Double = calcRegularizationScore(True)

				score_Conflict = 0.0
				Dim outNum As Integer = 0
				For Each s As String In configuration_Conflict.getNetworkOutputs()
					Dim gv As GraphVertex = verticesMap(s)
					If TypeOf gv Is LayerVertex Then
						'At this point: the input to the output layer might not be set on the layer itself - just the vertex
						Dim lv As LayerVertex = DirectCast(gv, LayerVertex)
						If Not lv.isSetLayerInput() Then
							lv.applyPreprocessorAndSetInput(workspaceMgr)
						End If
					End If
					Dim vertexLayer As Layer = gv.Layer
					If TypeOf vertexLayer Is FrozenLayerWithBackprop Then
						vertexLayer = DirectCast(vertexLayer, FrozenLayerWithBackprop).InsideLayer
					End If
					vertexLayer.MaskArray = If(labelMaskArrays_Conflict Is Nothing, Nothing, labelMaskArrays_Conflict(outNum))

					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						score_Conflict += DirectCast(vertexLayer, IOutputLayer).computeScore(r, True, workspaceMgr)
					End Using

					'Only want to add l1/l2 component once...
					r = 0.0
					outNum += 1
				Next s

				'Listeners
				If trainingListeners.Count > 0 Then
					Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						For Each tl As TrainingListener In trainingListeners
							tl.onBackwardPass(Me)
						Next tl
					End Using
				End If
			End Using

			For Each gv As GraphVertex In vertices_Conflict
				gv.clear()
			Next gv
		End Sub


		''' <summary>
		''' Conduct forward pass using a single input array. Note that this method can only be used with ComputationGraphs
		''' with a single input array.
		''' </summary>
		''' <param name="input"> The input array </param>
		''' <param name="layerTillIndex"> the layer to feed forward to </param>
		''' <param name="train"> If true: do forward pass at training time </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal input As INDArray, ByVal layerTillIndex As Integer, ByVal train As Boolean) As IDictionary(Of String, INDArray)
			If numInputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot feedForward with single input for graph network with " & numInputArrays_Conflict & " expected inputs")
			End If
			setInput(0, input)
			Return feedForward(train,layerTillIndex)
		End Function



		''' <summary>
		''' Conduct forward pass using an array of inputs. This overload allows the forward pass to be conducted, optionally
		''' (not) clearing the layer input arrays.<br>
		''' Note: when using clearInputs=false, there can be some performance and memory overhead: this is because the arrays are
		''' defined outside of workspaces (which are enabled by default) - otherwise, old/invalidated arrays could still be
		''' accessed after calling this method. Consequently: Don't use clearInputs=false unless you have a use case that
		''' requires them to remain after feed-forward has been completed
		''' </summary>
		''' <param name="input"> An array of ComputationGraph inputs </param>
		''' <param name="layerTillIndex"> the index of the layer to feed forward to </param>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <param name="clearInputs"> If true (default for other methods): clear the inputs of all layers after doing forward
		'''                    pass. False don't clear layer inputs. </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal input() As INDArray, ByVal layerTillIndex As Integer, ByVal train As Boolean, ByVal clearInputs As Boolean) As IDictionary(Of String, INDArray)
			Inputs = input
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, layerTillIndex, Nothing, input, inputMaskArrays_Conflict, labelMaskArrays_Conflict, clearInputs)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function


		''' <summary>
		''' Conduct forward pass using an array of inputs
		''' </summary>
		''' <param name="input"> An array of ComputationGraph inputs </param>
		''' <param name="layerTillIndex"> the index of the layer to feed forward to </param>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal input() As INDArray, ByVal layerTillIndex As Integer, ByVal train As Boolean) As IDictionary(Of String, INDArray)
			Inputs = input
			Return feedForward(train, layerTillIndex)
		End Function


		''' <summary>
		''' Conduct forward pass using the stored inputs
		''' </summary>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <param name="layerTillIndex"> the index of the layer to feed forward to </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal train As Boolean, ByVal layerTillIndex As Integer) As IDictionary(Of String, INDArray)
			Dim graphVertexIndexOfLayer As Integer = layers_Conflict(layerTillIndex).Index
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, graphVertexIndexOfLayer, Nothing, inputs_Conflict, inputMaskArrays_Conflict, labelMaskArrays_Conflict, True)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function



		''' <summary>
		''' Conduct forward pass using a single input array. Note that this method can only be used with ComputationGraphs
		''' with a single input array.
		''' </summary>
		''' <param name="input"> The input array </param>
		''' <param name="train"> If true: do forward pass at training time </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal input As INDArray, ByVal train As Boolean) As IDictionary(Of String, INDArray)
			If numInputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot feedForward with single input for graph network with " & numInputArrays_Conflict & " expected inputs")
			End If
			setInput(0, input)
			Return feedForward(train)
		End Function

		''' <summary>
		''' Conduct forward pass using an array of inputs
		''' </summary>
		''' <param name="input"> An array of ComputationGraph inputs </param>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal input() As INDArray, ByVal train As Boolean) As IDictionary(Of String, INDArray)
			Return feedForward(input, train, True)
		End Function

		''' <summary>
		''' Conduct forward pass using an array of inputs. This overload allows the forward pass to be conducted, optionally
		''' (not) clearing the layer input arrays.<br>
		''' Note: this method should NOT be used with clearInputs = true, unless you know what you are doing. Specifically:
		''' when using clearInputs=false, in combination with workspaces, the layer input fields may leak outside of the
		''' workspaces in which they were defined - potentially causing a crash. See <a href="https://deeplearning4j.konduit.ai/config/config-memory/config-workspaces">
		'''     https://deeplearning4j.konduit.ai/config/config-memory/config-workspaces</a>
		''' for more details
		''' </summary>
		''' <param name="input"> An array of ComputationGraph inputs </param>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <param name="clearInputs"> If true (default for other methods): clear the inputs of all layers after doing forward
		'''                    pass. False don't clear layer inputs. </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal input() As INDArray, ByVal train As Boolean, ByVal clearInputs As Boolean) As IDictionary(Of String, INDArray)
			Inputs = input
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, vertices_Conflict.Length - 1, Nothing, input, inputMaskArrays_Conflict, labelMaskArrays_Conflict, clearInputs)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Conduct forward pass using the stored inputs, at test time
		''' </summary>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward() As IDictionary(Of String, INDArray)
			Return feedForward(False)
		End Function

		''' <summary>
		''' Conduct forward pass using the stored inputs
		''' </summary>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <returns> A map of activations for each layer (not each GraphVertex). Keys = layer name, values = layer activations </returns>
		Public Overridable Function feedForward(ByVal train As Boolean) As IDictionary(Of String, INDArray)
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, vertices_Conflict.Length - 1, Nothing, inputs_Conflict, inputMaskArrays_Conflict, labelMaskArrays_Conflict, True)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <param name="train">                            True: training time. False: test time </param>
		''' <param name="excludeOutputLayers">              Should we exclude the output layers during forward pass? (usually: false) </param>
		''' <param name="includeNonLayerVertexActivations"> Include non-layer vertices in the output may? </param>
		''' <returns> Map of activations. Key: vertex name. Value: activations. </returns>
		Public Overridable Function feedForward(ByVal train As Boolean, ByVal excludeOutputLayers As Boolean, ByVal includeNonLayerVertexActivations As Boolean) As IDictionary(Of String, INDArray)
			Dim exclude() As Integer = Nothing
			If excludeOutputLayers Then
				exclude = OutputLayerIndices
			End If

			Dim m As IDictionary(Of String, INDArray) = ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, vertices_Conflict.Length-1, exclude, inputs_Conflict, inputMaskArrays_Conflict, labelMaskArrays_Conflict, True)
			If includeNonLayerVertexActivations Then
				Return m
			Else
				'Include only layers - in previous versions, we've always included inputs too for this method...
				Dim [out] As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
				For Each e As KeyValuePair(Of String, INDArray) In m.SetOfKeyValuePairs()
					Dim v As GraphVertex = verticesMap(e.Key)
					If TypeOf v Is LayerVertex OrElse TypeOf v Is InputVertex Then
						[out](e.Key) = e.Value
					End If
				Next e
				Return [out]
			End If
		End Function

		''' <summary>
		''' Return an array of network outputs (predictions) at test time, given the specified network inputs
		''' Network outputs are for output layers only.
		''' </summary>
		''' <param name="input"> Inputs to the network </param>
		''' <returns> Output activations (order: same as defined in network configuration) </returns>
		Public Overridable Function output(ParamArray ByVal input() As INDArray) As INDArray()
			Return output(False, input, inputMaskArrays_Conflict, labelMaskArrays_Conflict)
		End Function

		''' <summary>
		''' A convenience method that returns a single INDArray, instead of an INDArray[].
		''' Useful for ComputationGraphs that have only a single output.
		''' Otherwise identical to <seealso cref="output(INDArray...)"/>
		''' </summary>
		''' <param name="input"> Inputs to the network </param>
		''' <returns> Output activations array </returns>
		Public Overridable Function outputSingle(ParamArray ByVal input() As INDArray) As INDArray
			Return outputSingle(False, input)
		End Function

		''' <summary>
		''' Return an array of network outputs (predictions), given the specified network inputs
		''' Network outputs are for output layers only.
		''' </summary>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <param name="input"> Inputs to the network </param>
		''' <returns> Output activations (order: same as defined in network configuration) </returns>
		Public Overridable Function output(ByVal train As Boolean, ParamArray ByVal input() As INDArray) As INDArray()
			Return output(train, DirectCast(Nothing, MemoryWorkspace), input)
		End Function

		''' <summary>
		''' Return an array of network outputs (predictions), given the specified network inputs
		''' Network outputs are for output layers only.<br>
		''' If no memory workspace is provided, the output will be detached (not in any workspace).<br>
		''' If a memory workspace is provided, the output activation array (i.e., the INDArray returned by this method)
		''' will be placed in the specified workspace. This workspace must be opened by the user before calling this method -
		''' and the user is responsible for (a) closing this workspace, and (b) ensuring the output array is not used out
		''' of scope (i.e., not used after closing the workspace to which it belongs - as this is likely to cause either
		''' an exception when used, or a crash).
		''' </summary>
		''' <param name="train">           If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <param name="outputWorkspace"> May be null. If not null: the workspace MUST be opened before calling this method. </param>
		''' <param name="input">           Inputs to the network </param>
		''' <returns> Output activations (order: same as defined in network configuration) </returns>
		Public Overridable Function output(ByVal train As Boolean, ByVal outputWorkspace As MemoryWorkspace, ParamArray ByVal input() As INDArray) As INDArray()
			Return output(train, input, inputMaskArrays_Conflict, labelMaskArrays_Conflict, outputWorkspace)
		End Function

		''' <summary>
		''' Return an array of network outputs (predictions), given the specified network inputs
		''' Network outputs are for output layers only.
		''' </summary>
		''' <param name="train">      If true: forward pass for training mode. False: test mode </param>
		''' <param name="input">      Input arrays to the netwonk </param>
		''' <param name="inputMasks"> Optional input mask arrays (may be null) </param>
		''' <returns>           Network output activations </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray[] output(boolean train, @NonNull INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks)
		Public Overridable Function output(ByVal train As Boolean, ByVal input() As INDArray, ByVal inputMasks() As INDArray) As INDArray()
			Return output(train, input, inputMasks, Nothing)
		End Function

		''' <summary>
		''' Return an array of network outputs (predictions), given the specified network inputs
		''' Network outputs are for output layers only.
		''' </summary>
		''' <param name="train">      If true: forward pass for training mode. False: test mode </param>
		''' <param name="input">      Input arrays to the netwonk </param>
		''' <param name="inputMasks"> Optional input mask arrays (may be null) </param>
		''' <param name="labelMasks"> Optional label mask arrays (may be null </param>
		''' <returns>           Network output activations </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray[] output(boolean train, @NonNull INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks, org.nd4j.linalg.api.ndarray.INDArray[] labelMasks)
		Public Overridable Function output(ByVal train As Boolean, ByVal input() As INDArray, ByVal inputMasks() As INDArray, ByVal labelMasks() As INDArray) As INDArray()
			Return output(train, input, inputMasks, labelMasks, Nothing)
		End Function

		''' <summary>
		''' This method uses provided OutputAdapter to return custom object built from INDArray
		''' 
		''' PLEASE NOTE: This method uses dedicated Workspace for output generation to avoid redundant allocations
		''' </summary>
		''' <param name="inputs"> Input arrays to the netwonk </param>
		''' <param name="inputMasks"> Optional input mask arrays (may be null) </param>
		''' <param name="labelMasks"> Optional label mask arrays (may be null </param>
		''' <param name="outputAdapter"> OutputAdapter<T> instance </param>
		''' @param <T> T extends Object </param>
		''' <returns> T instance produced by OutputAdapter </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized <T> T output(@NonNull INDArray[] inputs, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks, org.nd4j.linalg.api.ndarray.INDArray[] labelMasks, @NonNull OutputAdapter<T> outputAdapter)
		Public Overridable Function output(Of T)(ByVal inputs() As INDArray, ByVal inputMasks() As INDArray, ByVal labelMasks() As INDArray, ByVal outputAdapter As OutputAdapter(Of T)) As T
			SyncLock Me
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(WS_ALL_LAYERS_ACT_CONFIG, WS_OUTPUT_MEM)
					If TypeOf outputAdapter Is ModelAdapter Then
						Return CType(outputAdapter, ModelAdapter(Of T)).apply(Me, inputs, inputMasks, labelMasks)
					Else
						Return outputAdapter.apply(output(False, inputs, inputMasks, labelMasks, ws))
					End If
				End Using
			End SyncLock
		End Function

		''' <summary>
		''' Return an array of network outputs (predictions), given the specified network inputs
		''' Network outputs are for output layers only.<br>
		''' If no memory workspace is provided, the output will be detached (not in any workspace).<br>
		''' If a memory workspace is provided, the output activation array (i.e., the INDArray returned by this method)
		''' will be placed in the specified workspace. This workspace must be opened by the user before calling this method -
		''' and the user is responsible for (a) closing this workspace, and (b) ensuring the output array is not used out
		''' of scope (i.e., not used after closing the workspace to which it belongs - as this is likely to cause either
		''' an exception when used, or a crash).
		''' </summary>
		''' <param name="train">           If true: forward pass for training mode. False: test mode </param>
		''' <param name="input">           Input arrays to the netwonk </param>
		''' <param name="inputMasks">      Optional input mask arrays (may be null) </param>
		''' <param name="labelMasks">      Optional label mask arrays (may be null </param>
		''' <param name="outputWorkspace"> May be null. If not null: the workspace MUST be opened before calling this method. </param>
		''' <returns> Network output activations </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public synchronized org.nd4j.linalg.api.ndarray.INDArray[] output(boolean train, @NonNull INDArray[] input, org.nd4j.linalg.api.ndarray.INDArray[] inputMasks, org.nd4j.linalg.api.ndarray.INDArray[] labelMasks, org.nd4j.linalg.api.memory.MemoryWorkspace outputWorkspace)
		Public Overridable Function output(ByVal train As Boolean, ByVal input() As INDArray, ByVal inputMasks() As INDArray, ByVal labelMasks() As INDArray, ByVal outputWorkspace As MemoryWorkspace) As INDArray()
			SyncLock Me
				Try
					setLayerMaskArrays(inputMasks, labelMasks)
					Dim [out]() As INDArray = outputOfLayersDetached(train, FwdPassType.STANDARD, OutputLayerIndices, input, inputMasks, labelMasks, True, False, outputWorkspace)
					clearLayerMaskArrays()
					clearLayersStates()
					Return [out]
				Catch e As System.OutOfMemoryException
					CrashReportingUtil.writeMemoryCrashDump(Me, e)
					Throw e
				End Try
			End SyncLock
		End Function

		''' <summary>
		''' A convenience method that returns a single INDArray, instead of an INDArray[].
		''' Useful for ComputationGraphs that have only a single output.
		''' Otherwise identical to <seealso cref="output(Boolean, INDArray...)"/>
		''' </summary>
		''' <param name="train"> If true: do forward pass at training time; false: do forward pass at test time </param>
		''' <param name="input"> Inputs to the network </param>
		''' <returns> Output activations array </returns>
		Public Overridable Function outputSingle(ByVal train As Boolean, ParamArray ByVal input() As INDArray) As INDArray
			Return outputSingle(train, True, input)
		End Function

		''' <summary>
		''' Identical to <seealso cref="outputSingle(Boolean, Boolean, INDArray...)"/> but has the option of not clearing the input
		''' arrays (useful when later backpropagating external errors). Most users should use <seealso cref="outputSingle(Boolean, INDArray...)"/>
		''' in preference to this method.
		''' </summary>
		Public Overridable Function outputSingle(ByVal train As Boolean, ByVal clearInputs As Boolean, ParamArray ByVal input() As INDArray) As INDArray
			If numOutputArrays_Conflict <> 1 Then
				Throw New System.InvalidOperationException("Cannot use outputSingle with ComputationGraph that does not have exactly 1 output. nOutputs: " & numOutputArrays_Conflict)
			End If
			Return output(train, clearInputs, input)(0)
		End Function

		''' <summary>
		''' An output method for the network, with optional clearing of the layer inputs.<br>
		''' Note: most users should use <seealso cref="output(Boolean, INDArray...)"/> or similar methods, unless they are doing
		''' non-standard operations (like providing the input arrays externally)
		''' </summary>
		''' <param name="train">       If true: output during training. False: output during testing. Affects some things such as
		'''                    dropout </param>
		''' <param name="clearInputs"> If true: clear the input arrays for all layers. False: leave the input arrays as-is - which
		'''                    can be useful for "external errors" (no output layer) backprop use cases </param>
		''' <param name="input">       Input to the network </param>
		''' <returns>            Output from the network </returns>
		Public Overridable Function output(ByVal train As Boolean, ByVal clearInputs As Boolean, ParamArray ByVal input() As INDArray) As INDArray()
			SyncLock Me
				Dim detachedInputs As Boolean = Not clearInputs 'If !clearInputs, then inputs should be detached (otherwise: will be out of scope)
				Try
					Return outputOfLayersDetached(train, FwdPassType.STANDARD, OutputLayerIndices, input, Nothing, Nothing, clearInputs, detachedInputs, Nothing)
				Catch e As System.OutOfMemoryException
					CrashReportingUtil.writeMemoryCrashDump(Me, e)
					Throw e
				End Try
			End SyncLock
		End Function

		''' <summary>
		''' Generate the output for all examples/batches in the input iterator, and concatenate them into a single array
		''' per network output
		''' </summary>
		''' <param name="iterator"> Data to pass through the network </param>
		''' <returns> output for all examples in the iterator </returns>
		Public Overridable Function output(ByVal iterator As DataSetIterator) As INDArray()
			Return output(New MultiDataSetIteratorAdapter(iterator))
		End Function

		''' <summary>
		''' Generate the output for all examples/batches in the input iterator, and concatenate them into a single array
		''' per network output
		''' </summary>
		''' <param name="iterator"> Data to pass through the network </param>
		''' <returns> output for all examples in the iterator </returns>
		Public Overridable Function output(ByVal iterator As MultiDataSetIterator) As INDArray()
			Dim outputs As IList(Of INDArray()) = New List(Of INDArray())()
			Do While iterator.MoveNext()
				Dim [next] As MultiDataSet = iterator.Current
				Dim [out]() As INDArray = output(False, [next].Features, [next].FeaturesMaskArrays, [next].LabelsMaskArrays)
				outputs.Add([out])
			Loop
			Dim arr()() As INDArray = CType(outputs, List(Of INDArray())).ToArray()
			Return DataSetUtil.mergeFeatures(arr, Nothing).First
		End Function

		''' <summary>
		''' Generate the output for all examples/batches in the input iterator, and concatenate them into a single array.
		''' Can only be used with ComputationGraphs with 1 output
		''' </summary>
		''' <param name="iterator"> Data to pass through the network </param>
		''' <returns> output for all examples in the iterator </returns>
		Public Overridable Function outputSingle(ByVal iterator As DataSetIterator) As INDArray
			Preconditions.checkArgument(numOutputArrays_Conflict = 1, "Cannot use this method with nets that have more" & " than 1 output array. This network has %s outputs", numOutputArrays_Conflict)
			Return output(iterator)(0)
		End Function

		''' <summary>
		''' Generate the output for all examples/batches in the input iterator, and concatenate them into a single array.
		''' Can only be used with ComputationGraphs with 1 output
		''' </summary>
		''' <param name="iterator"> Data to pass through the network </param>
		''' <returns> output for all examples in the iterator </returns>
		Public Overridable Function outputSingle(ByVal iterator As MultiDataSetIterator) As INDArray
			Preconditions.checkArgument(numOutputArrays_Conflict = 1, "Cannot use this method with nets that have more" & " than 1 output array. This network has %s outputs", numOutputArrays_Conflict)
			Return output(iterator)(0)
		End Function

		''' <summary>
		''' Get the activations for the specific layers only </summary>
		''' <param name="layers">       Layers to get the specified activations for </param>
		''' <param name="train">        If true: train mode. False: test (inference) mode </param>
		''' <param name="features">     Features array </param>
		''' <param name="featureMasks"> Feature masks array. May be null </param>
		''' <returns> Activations of the selected layers, in the same order as the "layers" arg/list </returns>
		Public Overridable Function output(ByVal layers As IList(Of String), ByVal train As Boolean, ByVal features() As INDArray, ByVal featureMasks() As INDArray) As INDArray()
			Preconditions.checkState(layers IsNot Nothing AndAlso layers.Count > 0, "Layers must not be null: got later names %s", layers)
			Dim layerNums(layers.Count - 1) As Integer
			For i As Integer = 0 To layers.Count - 1
				Dim n As String = layers(i)
				Preconditions.checkState(verticesMap.ContainsKey(n), "Layer with name %s not found in network", n)
				layerNums(i) = verticesMap(n).getVertexIndex()
			Next i
			Dim [out]() As INDArray = outputOfLayersDetached(train, FwdPassType.STANDARD, layerNums, features, featureMasks, Nothing, True, False, Nothing)
			Return [out]
		End Function


		Protected Friend Overridable Sub validateArrayWorkspaces(ByVal mgr As LayerWorkspaceMgr, ByVal array As INDArray, ByVal arrayType As ArrayType, ByVal vertexName As String, ByVal isInputVertex As Boolean, ByVal op As String)
			Try
				mgr.validateArrayLocation(arrayType, array, False, isInputVertex)
			Catch e As ND4JWorkspaceException
				Dim clazz As String
				Dim v As GraphVertex = verticesMap(vertexName)
				If TypeOf v Is LayerVertex Then
					clazz = v.Layer.GetType().Name
				Else
					clazz = v.GetType().Name
				End If
				Throw New System.InvalidOperationException(op & ": array (" & arrayType & ") workspace validation failed (vertex " & vertexName & " - class: " & clazz & ") - array is defined in incorrect workspace", e)
			End Try
		End Sub

		''' <summary>
		''' Feed-forward through the network - returning all array activations detached from any workspace.
		''' Note that no workspace should be active externally when calling this method (an exception will be thrown
		''' if a workspace is open externally)
		''' </summary>
		''' <param name="train">             Training mode (true) or test/inference mode (false) </param>
		''' <param name="fwdPassType">       Type of forward pass to perform (STANDARD or RNN_ACTIVATE_WITH_STORED_STATE only) </param>
		''' <param name="storeLastForTBPTT"> ONLY used if fwdPassType == FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE </param>
		''' <param name="layerIndex">        Index (inclusive) to stop forward pass at. For all layers, use numLayers-1 </param>
		''' <param name="excludeIdxs">       Layers (vertices) to exclude from forward pass. These layers will be skipped, and hence
		'''                          are usually output layers or at the end of the network. May be null. </param>
		''' <param name="features">          Input feature arrays </param>
		''' <param name="fMask">             Feature mask arrays. May be null. </param>
		''' <param name="lMask">             Label mask array. May be null. </param>
		''' <param name="clearLayers">       Whether the layer inputs should be cleared </param>
		''' <returns> Map of activations (including the input), detached from any workspace </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected synchronized Map<String,org.nd4j.linalg.api.ndarray.INDArray> ffToLayerActivationsDetached(boolean train, @NonNull FwdPassType fwdPassType, boolean storeLastForTBPTT, int layerIndex, int[] excludeIdxs, @NonNull INDArray[] features, org.nd4j.linalg.api.ndarray.INDArray[] fMask, org.nd4j.linalg.api.ndarray.INDArray[] lMask, boolean clearLayers)
		Protected Friend Overridable Function ffToLayerActivationsDetached(ByVal train As Boolean, ByVal fwdPassType As FwdPassType, ByVal storeLastForTBPTT As Boolean, ByVal layerIndex As Integer, ByVal excludeIdxs() As Integer, ByVal features() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal clearLayers As Boolean) As IDictionary(Of String, INDArray)
			SyncLock Me
				If layerIndex < 0 OrElse layerIndex >= topologicalOrder.Length Then
					Throw New System.ArgumentException("Invalid layer index - index must be >= 0 and < " & topologicalOrder.Length & ", got index " & layerIndex)
				End If
        
				Inputs = features
				setLayerMaskArrays(fMask, lMask)
        
				'Verify that no workspace is open externally
				WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active before call to ffToLayerActivationsDetached", True)
        
				Dim workspaceMgr As LayerWorkspaceMgr
				Dim wsm As WorkspaceMode = (If(train, configuration_Conflict.getTrainingWorkspaceMode(), configuration_Conflict.getInferenceWorkspaceMode()))
				If wsm = WorkspaceMode.NONE Then
					workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
				Else
					workspaceMgr = LayerWorkspaceMgr.builder().noWorkspaceFor(ArrayType.ACTIVATIONS).with(ArrayType.INPUT, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
        
					If features(0).isAttached() Then
						'Don't leverage out of async DataMultiSetIterator workspaces
						workspaceMgr.NoLeverageOverride = features(0).data().getParentWorkspace().getId()
					End If
				End If
				workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)
        
				Dim activations As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
        
				'Add the inputs:
				For i As Integer = 0 To features.Length - 1
					activations(configuration_Conflict.getNetworkInputs().get(i)) = features(i)
				Next i
        
				Dim traceLog As Boolean = log.isTraceEnabled()
        
				'Do forward pass according to the topological ordering of the network
				For i As Integer = 0 To layerIndex
					Dim current As GraphVertex = vertices_Conflict(topologicalOrder(i))
					Dim vName As String = current.VertexName
					Dim vIdx As Integer = current.VertexIndex
        
					If excludeIdxs IsNot Nothing AndAlso ArrayUtils.contains(excludeIdxs, vIdx) Then
						Continue For
					End If
        
					If traceLog Then
						log.trace("About forward pass: {} (""{}"") - {}", i, vName, current.GetType().Name)
					End If
        
					Using wsFFWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						Dim inputsTo() As VertexIndices = current.OutputVertices
        
						Dim [out] As INDArray
						If current.InputVertex Then
							[out] = inputs_Conflict(vIdx)
						Else
        
							If fwdPassType = FwdPassType.STANDARD Then
								'Standard feed-forward case
								[out] = current.doForward(train, workspaceMgr)
							ElseIf fwdPassType = FwdPassType.RNN_TIMESTEP Then
								If current.hasLayer() Then
									'Layer
									Dim input As INDArray = current.Inputs(0)
									Dim l As Layer = current.Layer
									If TypeOf l Is RecurrentLayer Then
										[out] = DirectCast(l, RecurrentLayer).rnnTimeStep(reshapeTimeStepInput(input), workspaceMgr)
									ElseIf TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer AndAlso TypeOf (DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
										Dim rl As RecurrentLayer = (DirectCast(DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer).getUnderlying(), RecurrentLayer))
										[out] = rl.rnnTimeStep(reshapeTimeStepInput(input), workspaceMgr)
									ElseIf TypeOf l Is MultiLayerNetwork Then
										[out] = DirectCast(l, MultiLayerNetwork).rnnTimeStep(reshapeTimeStepInput(input))
									Else
										'non-recurrent layer
										[out] = current.doForward(train, workspaceMgr)
									End If
								Else
									'GraphNode
									[out] = current.doForward(train, workspaceMgr)
								End If
							ElseIf fwdPassType = FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE Then
								If current.hasLayer() Then
									Dim l As Layer = current.Layer
									If TypeOf l Is RecurrentLayer Then
										[out] = DirectCast(l, RecurrentLayer).rnnActivateUsingStoredState(current.Inputs(0), train, storeLastForTBPTT, workspaceMgr)
									ElseIf TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer AndAlso TypeOf (DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
										Dim rl As RecurrentLayer = DirectCast((DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer)).getUnderlying(), RecurrentLayer)
										[out] = rl.rnnActivateUsingStoredState(current.Inputs(0), train,storeLastForTBPTT, workspaceMgr)
									ElseIf TypeOf l Is MultiLayerNetwork Then
										Dim temp As IList(Of INDArray) = DirectCast(l, MultiLayerNetwork).rnnActivateUsingStoredState(current.Inputs(0), train, storeLastForTBPTT)
										[out] = temp(temp.Count - 1)
									Else
										'non-recurrent layer
										[out] = current.doForward(train, workspaceMgr)
									End If
								Else
									[out] = current.doForward(train, workspaceMgr)
								End If
							Else
								Throw New System.ArgumentException("Unsupported forward pass type for this method: " & fwdPassType)
							End If
							validateArrayWorkspaces(workspaceMgr, [out], ArrayType.ACTIVATIONS, vName, False, "Feed forward (inference)")
						End If
						activations(current.VertexName) = [out]
        
						If inputsTo IsNot Nothing Then 'May be null for output vertices (which don't feed into any other vertices)
							For Each v As VertexIndices In inputsTo
								'Note that we don't have to do anything special here: the activations are always detached in
								' this method
								Dim inputToIndex As Integer = v.VertexIndex
								Dim vIdxEdge As Integer = v.VertexEdgeNumber
								vertices_Conflict(inputToIndex).setInput(vIdxEdge, [out], workspaceMgr)
							Next v
						End If
        
						If clearLayers Then
							current.clear()
						End If
					End Using
        
					If traceLog Then
						log.trace("Completed forward pass: {} (""{}"") - {}", i, vName, current.GetType().Name)
					End If
				Next i
        
				Return activations
			End SyncLock
		End Function

		''' <summary>
		''' Feed-forward through the network - if workspaces are used, all returned activations will be present in workspace
		''' WS_ALL_LAYERS_ACT.<br>
		''' Note: if using workspaces for training, requires that WS_ALL_LAYERS_ACT is open externally.
		''' If using NO workspaces, requires that no external workspace is open
		''' </summary>
		''' <param name="train">             Training mode (true) or test/inference mode (false) </param>
		''' <param name="layerIndex">        Index (inclusive) to stop forward pass at. For all layers, use -1 </param>
		''' <param name="excludeIdxs">       Layers (vertices) to exclude from forward pass. These layers will be skipped, and hence
		'''                          are usually output layers or at the end of the network. May be null. </param>
		''' <param name="fwdPassType">       Type of forward pass to perform (STANDARD or RNN_ACTIVATE_WITH_STORED_STATE only) </param>
		''' <param name="storeLastForTBPTT"> ONLY used if fwdPassType == FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE </param>
		''' <param name="input">             Input feature arrays </param>
		''' <param name="fMask">             Feature mask arrays. May be null. </param>
		''' <param name="lMask">             Label mask array. May be null. </param>
		''' <param name="clearInputs">       Whether the layer inputs should be cleared </param>
		''' <returns> Map of activations (including the input), in workspace WS_ALL_LAYERS_ACT if workspaces are used (detached
		''' otherwise) </returns>
		Protected Friend Overridable Function ffToLayerActivationsInWS(ByVal train As Boolean, ByVal layerIndex As Integer, ByVal excludeIdxs() As Integer, ByVal fwdPassType As FwdPassType, ByVal storeLastForTBPTT As Boolean, ByVal input() As INDArray, ByVal fMask() As INDArray, ByVal lMask() As INDArray, ByVal clearInputs As Boolean) As IDictionary(Of String, INDArray)
			SyncLock Me
				If layerIndex <> -1 AndAlso (layerIndex < 0 OrElse layerIndex >= topologicalOrder.Length) Then
					Throw New System.ArgumentException("Invalid input index - index must be >= 0 and < " & topologicalOrder.Length & ", got index " & layerIndex)
				End If
				Inputs = input
				setLayerMaskArrays(fMask, lMask)
        
				Dim workspaceMgr As LayerWorkspaceMgr
				Dim wsm As WorkspaceMode = (If(train, configuration_Conflict.getTrainingWorkspaceMode(), configuration_Conflict.getInferenceWorkspaceMode()))
				If wsm = WorkspaceMode.NONE Then
					'Verify that no workspace is open externally
					WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active in ffToLayerActivationsDetached", True)
        
					workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
				Else
					WorkspaceUtils.assertOpenAndActive(WS_ALL_LAYERS_ACT, "ffToLayerActivationsInWs method requires workspace WS_ALL_LAYERS_ACT to be open")
        
					workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
        
					If input(0).Attached Then
						'Don't leverage out of async DataMultiSetIterator workspaces
						workspaceMgr.NoLeverageOverride = input(0).data().ParentWorkspace.Id
					End If
        
					If configuration_Conflict.getCacheMode() <> CacheMode.NONE Then
						'For now: store cache mode activations in activations workspace
						workspaceMgr.setWorkspace(ArrayType.FF_CACHE, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG)
					End If
				End If
				workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)
        
				Dim traceLog As Boolean = log.isTraceEnabled()
        
				Dim activations As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
				'Do forward pass according to the topological ordering of the network
				Dim stopIndex As Integer
				If layerIndex > 0 Then
					stopIndex = ArrayUtils.IndexOf(topologicalOrder, layerIndex)
				Else
					stopIndex = topologicalOrder.Length -1
				End If
				For i As Integer = 0 To stopIndex
					Dim current As GraphVertex = vertices_Conflict(topologicalOrder(i))
					Dim vName As String = current.VertexName
					Dim vIdx As Integer = current.VertexIndex
        
					If traceLog Then
						log.trace("About forward pass: {} (""{}"") - {}", i, vName, current.GetType().Name)
					End If
        
					If excludeIdxs IsNot Nothing AndAlso ArrayUtils.contains(excludeIdxs, vIdx) Then
						Continue For
					End If
        
					Using wsFFWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						Dim inputsTo() As VertexIndices = current.OutputVertices
        
						Dim [out] As INDArray
						If current.InputVertex Then
							[out] = inputs_Conflict(vIdx)
						Else
        
							If fwdPassType = FwdPassType.STANDARD Then
								[out] = current.doForward(train, workspaceMgr)
							ElseIf fwdPassType = FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE Then
								If current.hasLayer() Then
									Dim l As Layer = current.Layer
									If TypeOf l Is RecurrentLayer Then
										[out] = DirectCast(l, RecurrentLayer).rnnActivateUsingStoredState(current.Inputs(0), train, storeLastForTBPTT, workspaceMgr)
									ElseIf TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer AndAlso TypeOf (DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
										Dim rl As RecurrentLayer = DirectCast((DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer)).getUnderlying(), RecurrentLayer)
										[out] = rl.rnnActivateUsingStoredState(current.Inputs(0), train,storeLastForTBPTT, workspaceMgr)
									ElseIf TypeOf l Is MultiLayerNetwork Then
										Dim temp As IList(Of INDArray) = DirectCast(l, MultiLayerNetwork).rnnActivateUsingStoredState(current.Inputs(0), train, storeLastForTBPTT)
										[out] = temp(temp.Count - 1)
									Else
										'non-recurrent layer
										[out] = current.doForward(train, workspaceMgr)
									End If
								Else
									[out] = current.doForward(train, workspaceMgr)
								End If
							Else
								Throw New System.InvalidOperationException("FwdPassType not supported for this method: " & fwdPassType)
							End If
        
							validateArrayWorkspaces(workspaceMgr, [out], ArrayType.ACTIVATIONS, vName, False, "Feed forward (inference)")
						End If
						activations(current.VertexName) = [out]
        
						If inputsTo IsNot Nothing Then
							'Can be null for output layers
							For Each v As VertexIndices In inputsTo
								'Note that we don't have to do anything special here: the activations are always detached in
								' this method
								Dim inputToIndex As Integer = v.VertexIndex
								Dim vIdxEdge As Integer = v.VertexEdgeNumber
								vertices_Conflict(inputToIndex).setInput(vIdxEdge, [out], workspaceMgr)
							Next v
						End If
        
						If clearInputs Then
							current.clear()
						End If
					End Using
        
					If traceLog Then
						log.trace("Completed forward pass: {} (""{}"") - {}", i, vName, current.GetType().Name)
					End If
				Next i
				Return activations
			End SyncLock
		End Function


		''' <summary>
		''' Provide the output of the specified layers, detached from any workspace. This is most commonly used at inference/test
		''' time, and is more memory efficient than <seealso cref="ffToLayerActivationsDetached(Boolean, FwdPassType, Boolean, Integer, Integer[], INDArray[], INDArray[], INDArray[], Boolean)"/>
		''' and <seealso cref="ffToLayerActivationsInWS(Boolean, Integer, Integer[], FwdPassType, Boolean, INDArray[], INDArray[], INDArray[], Boolean)"/>.<br>
		''' This method clears all layer inputs.
		''' 
		''' NOTE: in general, no workspaces should be activated externally for this method!
		''' This method handles the workspace activation as required
		''' </summary>
		''' <param name="train">             Training mode (true) or test/inference mode (false) </param>
		''' <param name="fwdPassType">       Type of forward pass to perform (STANDARD or RNN_TIMESTEP only) </param>
		''' <param name="layerIndexes">      Indexes of the layers to get the activations for </param>
		''' <param name="features">          Input features for the network </param>
		''' <param name="fMask">             Input/feature mask array. May be null. </param>
		''' <param name="lMasks">            Labels mask array. May be null </param>
		''' <param name="clearLayerInputs">  If true: the layer input fields will be cleared </param>
		''' <param name="detachedInputs">    If true: the layer input fields will be detached. Usually used for external errors cases </param>
		''' <param name="outputWorkspace">   Optional - if provided, outputs should be placed in this workspace. NOTE: this workspace
		'''                          must be open </param>
		''' <returns>                  Output of the specified layers, detached from any workspace </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.nd4j.linalg.api.ndarray.INDArray[] outputOfLayersDetached(boolean train, @NonNull FwdPassType fwdPassType, @NonNull int[] layerIndexes, @NonNull INDArray[] features, org.nd4j.linalg.api.ndarray.INDArray[] fMask, org.nd4j.linalg.api.ndarray.INDArray[] lMasks, boolean clearLayerInputs, boolean detachedInputs, org.nd4j.linalg.api.memory.MemoryWorkspace outputWorkspace)
		Protected Friend Overridable Function outputOfLayersDetached(ByVal train As Boolean, ByVal fwdPassType As FwdPassType, ByVal layerIndexes() As Integer, ByVal features() As INDArray, ByVal fMask() As INDArray, ByVal lMasks() As INDArray, ByVal clearLayerInputs As Boolean, ByVal detachedInputs As Boolean, ByVal outputWorkspace As MemoryWorkspace) As INDArray()
			If features.Length <> numInputArrays_Conflict Then
				Throw New System.ArgumentException("Invalid number of input arrays: network has " & numInputArrays_Conflict & " inputs, got " & features.Length & " input arrays")
			End If
			For i As Integer = 0 To layerIndexes.Length - 1
				If layerIndexes(i) < 0 OrElse layerIndexes(i) >= topologicalOrder.Length Then
					Throw New System.ArgumentException("Invalid input index - index must be >= 0 and < " & topologicalOrder.Length & ", got index " & layerIndexes(i))
				End If
			Next i
			Inputs = features
			setLayerMaskArrays(fMask, lMasks)

			Dim outputPrevious As MemoryWorkspace = Nothing
			If outputWorkspace Is Nothing OrElse TypeOf outputWorkspace Is DummyWorkspace Then
				'Verify that no workspace is open externally
				WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active before call to outputOfLayersDetached")
			Else
				Preconditions.checkState(outputWorkspace.ScopeActive, "Workspace """ & outputWorkspace.Id & """ was provided for the network/layer outputs. When provided, this workspace must be opened before " & "calling the output method; furthermore, closing the workspace is the responsibility of the user")
				outputPrevious = outputWorkspace.ParentWorkspace
			End If


			'First: for each vertex, determine the highest index of the vertex that consumes it's output
			'Then: for each vertex, determine the forward pass step that each vertex's output has been fully consumed on
			'In other words, if vertex X -> Y and X -> Z, and topological sort order is X,a,Y,b,Z,
			'Then we know X's output activations have been fully consumed by step index 4 in the topological sort
			'thus vertexOutputsFullyConsumedByStep[X.index] = IndexOf(topologicalSort, Z.index)

			'Position in array: index of vertex. Value at position: the step (in topological order) that the activations
			' have been consumed by
			'Put another way: this is the step that it's safe to deallocate the layer's activations by closing the
			' corresponding workspace
			Dim vertexOutputsFullyConsumedByStep(topologicalOrder.Length - 1) As Integer
			For Each gv As GraphVertex In vertices_Conflict
				Dim idx As Integer = gv.VertexIndex
				Dim maxStepOfOutputTo As Integer = -1
				Dim outputsTo() As VertexIndices = gv.OutputVertices
				If outputsTo IsNot Nothing Then
					'May be null for final/output layers
					For Each vi As VertexIndices In outputsTo
						Dim posInTopoSort As Integer = ArrayUtils.IndexOf(topologicalOrder, vi.VertexIndex)
						If posInTopoSort = -1 Then
							Throw New System.InvalidOperationException("Did not find vertex " & vi.VertexIndex & " in topological sort array")
						End If
						maxStepOfOutputTo = Math.Max(maxStepOfOutputTo, posInTopoSort)
					Next vi
				Else
					maxStepOfOutputTo = topologicalOrder.Length-1
				End If
				vertexOutputsFullyConsumedByStep(idx) = maxStepOfOutputTo
			Next gv

			'Do forward pass according to the topological ordering of the network
			Dim outputs(layerIndexes.Length - 1) As INDArray
			Dim stopIndex As Integer = -1
			For i As Integer = 0 To layerIndexes.Length - 1
				stopIndex = Math.Max(stopIndex, ArrayUtils.IndexOf(topologicalOrder, layerIndexes(i)))
			Next i
			Dim allWorkspaceManagers As IList(Of LayerWorkspaceMgr) = New List(Of LayerWorkspaceMgr)()
			Dim freeWorkspaceManagers As IList(Of LayerWorkspaceMgr) = New List(Of LayerWorkspaceMgr)() 'Basically used as a stack
			Dim openActivationsWorkspaces As IDictionary(Of MemoryWorkspace, LayerWorkspaceMgr) = New IdentityHashMap(Of MemoryWorkspace, LayerWorkspaceMgr)()

			Dim wsm As WorkspaceMode = (If(train, configuration_Conflict.getTrainingWorkspaceMode(), configuration_Conflict.getInferenceWorkspaceMode()))
			Dim noWS As Boolean = wsm = WorkspaceMode.NONE
			Dim allNone As LayerWorkspaceMgr = If(noWS, LayerWorkspaceMgr.noWorkspaces(helperWorkspaces), Nothing)
			Dim closeAtEndIteraton() As IList(Of MemoryWorkspace) = CType(New System.Collections.IList(topologicalOrder.Length - 1){}, IList(Of MemoryWorkspace)())
			Dim initialWorkspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			Dim t As Exception = Nothing
			Try
				For i As Integer = 0 To stopIndex
					Dim current As GraphVertex = vertices_Conflict(topologicalOrder(i))
					Dim prev As GraphVertex = If(i > 0, vertices_Conflict(topologicalOrder(i - 1)), Nothing)

					Dim vName As String = current.VertexName
					Dim vIdx As Integer = current.VertexIndex

					'First: determine what workspace manager we should use for forward pass in this vertex
					Dim workspaceMgr As LayerWorkspaceMgr
					If noWS Then
						workspaceMgr = allNone
					Else
						'First: is there a free forward pass workspace we can use?
						If freeWorkspaceManagers.Count > 0 Then
							workspaceMgr = freeWorkspaceManagers.RemoveAt(freeWorkspaceManagers.Count - 1)
						Else
							'No existing free workspace managers for forward pass - create a new one...
							Dim wsName As String = "WS_LAYER_ACT_" & allWorkspaceManagers.Count
							workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.INPUT, wsName, WS_LAYER_ACT_X_CONFIG).with(ArrayType.ACTIVATIONS, wsName, WS_LAYER_ACT_X_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

							If detachedInputs Then
								'Sometimes (like: external errors use cases) we don't want the activations/inputs to be
								' in a workspace
								workspaceMgr.ScopedOutFor = ArrayType.INPUT
								workspaceMgr.ScopedOutFor = ArrayType.ACTIVATIONS
							Else
								'Don't leverage out of async MultiDataSetIterator workspaces
								If features(0).isAttached() Then
									workspaceMgr.NoLeverageOverride = features(0).data().getParentWorkspace().getId()
								End If
							End If

							allWorkspaceManagers.Add(workspaceMgr)
						End If
					End If
					workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)

					'Is this one of the layers/vertices that we want the output for?
					Dim isRequiredOutput As Boolean = False
					Dim origWSAct As String = Nothing
					Dim origWSActConf As WorkspaceConfiguration = Nothing
					If ArrayUtils.contains(layerIndexes, vIdx) Then
						isRequiredOutput = True

						If outputWorkspace IsNot Nothing AndAlso Not (TypeOf outputWorkspace Is DummyWorkspace) Then
							'Place activations in user-specified workspace
							origWSAct = workspaceMgr.getWorkspaceName(ArrayType.ACTIVATIONS)
							origWSActConf = workspaceMgr.getConfiguration(ArrayType.ACTIVATIONS)
							workspaceMgr.setWorkspace(ArrayType.ACTIVATIONS, outputWorkspace.Id, outputWorkspace.WorkspaceConfiguration)
						Else
							'Standard case
							If Not workspaceMgr.isScopedOut(ArrayType.ACTIVATIONS) Then
								'Activations/output to return: don't want this in any workspace
								origWSAct = workspaceMgr.getWorkspaceName(ArrayType.ACTIVATIONS)
								origWSActConf = workspaceMgr.getConfiguration(ArrayType.ACTIVATIONS)
								workspaceMgr.ScopedOutFor = ArrayType.ACTIVATIONS
							End If
						End If
					End If

					'Open the relevant workspace for the activations.
					'Note that this will be closed only once the current vertex's activations have been consumed
					 Dim wsActivations As MemoryWorkspace = Nothing
					If outputWorkspace Is Nothing OrElse TypeOf outputWorkspace Is DummyWorkspace OrElse Not isRequiredOutput Then 'Open WS if (a) no external/output WS (if present, it's already open), or (b) not being placed in external/output WS
						wsActivations = workspaceMgr.notifyScopeEntered(ArrayType.ACTIVATIONS)
						openActivationsWorkspaces(wsActivations) = workspaceMgr
					End If

					'Note that because we're opening activation workspaces not in any defined order (i.e., workspace
					' use isn't simply nested), we'll manually override the previous workspace setting. Otherwise, when we
					' close these workspaces, the "current" workspace may be set to the incorrect one
					If wsActivations IsNot Nothing Then
						wsActivations.PreviousWorkspace = initialWorkspace
					End If

					Dim closeableAt As Integer = vertexOutputsFullyConsumedByStep(vIdx)
					If outputWorkspace Is Nothing OrElse TypeOf outputWorkspace Is DummyWorkspace OrElse (wsActivations IsNot Nothing AndAlso Not outputWorkspace.Id.Equals(wsActivations.Id)) Then
						If closeAtEndIteraton(closeableAt) Is Nothing Then
							closeAtEndIteraton(closeableAt) = New List(Of MemoryWorkspace)()
						End If
						closeAtEndIteraton(closeableAt).Add(wsActivations)
					End If


					Using wsFFWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						Dim inputsTo() As VertexIndices = current.OutputVertices

						Dim [out] As INDArray = Nothing
						If current.InputVertex Then
							[out] = features(vIdx)
						Else

							If fwdPassType = FwdPassType.STANDARD Then
								'Standard feed-forward case

								If i > 0 AndAlso current.hasLayer() AndAlso prev.hasLayer() AndAlso ConvolutionUtils.layerHasConvolutionLayout(prev.Layer.conf().getLayer()) AndAlso ConvolutionUtils.layerHasConvolutionLayout(current.Layer.conf().getLayer()) Then

									''' <summary>
									''' Not QUITE the proper fix, but getting close.
									''' Able to detect this happens mid graph and do something about it.
									''' Need to play with output sizes a bit to make sure we put the right parameters in there to get
									''' correct behavior.
									''' </summary>
									Dim preLayerFormat As CNN2DFormat = ConvolutionUtils.getFormatForLayer(prev.Layer.conf().getLayer())
									Dim currLayerFormat As CNN2DFormat = ConvolutionUtils.getFormatForLayer(current.Layer.conf().getLayer())
									If preLayerFormat <> currLayerFormat Then
										Dim inputIdx As Integer = -1
										Dim inputVertex As Integer = 0
										Do While inputVertex < current.InputVertices.Length
											If current.InputVertices(inputVertex).VertexIndex = prev.VertexIndex Then
												inputIdx = inputVertex
											End If
											inputVertex += 1
										Loop

										'NHWC case
										If preLayerFormat = CNN2DFormat.NCHW Then
											current.setInput(inputIdx,current.Inputs(inputIdx).permute(0,3,1,2),workspaceMgr)
										'NCHW case
										ElseIf preLayerFormat = CNN2DFormat.NHWC Then
											current.setInput(inputIdx,current.Inputs(inputIdx).permute(0,2,3,1),workspaceMgr)

										Else
											Throw New System.InvalidOperationException("No CNN2DDataFormat type found for previous layer!")
										End If

										[out] = current.doForward(train, workspaceMgr)
									Else
										[out] = current.doForward(train, workspaceMgr)
									End If
								ElseIf i > 0 AndAlso current.hasLayer() AndAlso prev.hasLayer() AndAlso Convolution1DUtils.hasRnnDataFormat(prev.Layer.conf().getLayer()) AndAlso Convolution1DUtils.hasRnnDataFormat(current.Layer.conf().getLayer()) Then
									Dim preLayerFormat As RNNFormat = Convolution1DUtils.getRnnFormatFromLayer(prev.Layer.conf().getLayer())
									Dim currLayerFormat As RNNFormat = Convolution1DUtils.getRnnFormatFromLayer(current.Layer.conf().getLayer())
									Dim inputIdx As Integer = -1
									Dim inputVertex As Integer = 0
									Do While inputVertex < current.InputVertices.Length
										If current.InputVertices(inputVertex).VertexIndex = prev.VertexIndex Then
											inputIdx = inputVertex
										End If
										inputVertex += 1
									Loop
									'permute for next layer
									If preLayerFormat <> currLayerFormat Then
										current.setInput(inputIdx,current.Inputs(inputIdx).permute(0,2,1),workspaceMgr)
									End If

									[out] = current.doForward(train, workspaceMgr)


								Else
									[out] = current.doForward(train, workspaceMgr)
								End If
							ElseIf fwdPassType = FwdPassType.RNN_TIMESTEP Then
								If current.hasLayer() Then
									'Layer
									Dim input As INDArray = current.Inputs(0)
									Dim l As Layer = current.Layer
									If TypeOf l Is RecurrentLayer Then
										[out] = DirectCast(l, RecurrentLayer).rnnTimeStep(reshapeTimeStepInput(input), workspaceMgr)
									ElseIf TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer AndAlso TypeOf (DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
										Dim rl As RecurrentLayer = (DirectCast(DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer).getUnderlying(), RecurrentLayer))
										[out] = rl.rnnTimeStep(reshapeTimeStepInput(input), workspaceMgr)
									ElseIf TypeOf l Is MultiLayerNetwork Then
										[out] = DirectCast(l, MultiLayerNetwork).rnnTimeStep(reshapeTimeStepInput(input))
									Else
										'non-recurrent layer
										[out] = current.doForward(train, workspaceMgr)
									End If
								Else
									'GraphNode
									[out] = current.doForward(train, workspaceMgr)
								End If
							Else
								Throw New System.ArgumentException("Unsupported forward pass type for this method: " & fwdPassType)
							End If
							validateArrayWorkspaces(workspaceMgr, [out], ArrayType.ACTIVATIONS, vName, False, "Feed forward (inference)")
						End If

						If inputsTo IsNot Nothing Then 'Output vertices may not input to any other vertices
							For Each v As VertexIndices In inputsTo
								'Note that we don't have to do anything special here: the activations are always detached in
								' this method
								Dim inputToIndex As Integer = v.VertexIndex
								Dim vIdxEdge As Integer = v.VertexEdgeNumber
								vertices_Conflict(inputToIndex).setInput(vIdxEdge, [out], workspaceMgr)
							Next v
						End If

						If clearLayerInputs Then
							current.clear()
						End If

						If isRequiredOutput Then
							outputs(ArrayUtils.IndexOf(layerIndexes, vIdx)) = [out]
							If origWSAct IsNot Nothing Then
								'Reset the configuration, as we may reuse this workspace manager...
								workspaceMgr.setWorkspace(ArrayType.ACTIVATIONS, origWSAct, origWSActConf)
							End If
						End If
					End Using

					'Close any activations workspaces that we no longer require
					'Note that activations workspaces can be closed only once the corresponding output activations have
					' been fully consumed
					If closeAtEndIteraton(i) IsNot Nothing Then
						For Each wsAct As MemoryWorkspace In closeAtEndIteraton(i)
							wsAct.close()
							Dim canNowReuse As LayerWorkspaceMgr = openActivationsWorkspaces.Remove(wsAct)
							freeWorkspaceManagers.Add(canNowReuse)
						Next wsAct
					End If
				Next i
			Catch t2 As Exception
				t = t2
			Finally
				'Close all open workspaces... usually this list will be empty, but not if an exception is thrown
				'Though if stopIndex < numLayers, some might still be open
				For Each ws As MemoryWorkspace In openActivationsWorkspaces.Keys
					Do While ws.ScopeActive
						'Edge case here: seems that scoping out can increase the tagScope of the current WS
						'and if we hit an exception during forward pass, we aren't guaranteed to call close a sufficient
						' number of times to actually close it, in all cases
						Try
							ws.close()
						Catch t2 As Exception
							If t IsNot Nothing Then
								log.error("Encountered second exception while trying to close workspace after initial exception")
								log.error("Original exception:", t)
								Throw t2
							End If
						End Try
					Loop
				Next ws
				Nd4j.MemoryManager.CurrentWorkspace = initialWorkspace

				If t IsNot Nothing Then
					If TypeOf t Is Exception Then
						Throw (CType(t, Exception))
					End If
					Throw New Exception("Error during neural network forward pass", t)
				End If

				If outputWorkspace Is Nothing OrElse TypeOf outputWorkspace Is DummyWorkspace Then
					WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active at the end of outputOfLayerDetached")
				Else
					Preconditions.checkState(outputWorkspace.ScopeActive, "Expected output workspace to still be open" & "at end of outputOfLayerDetached, but ")
					outputWorkspace.PreviousWorkspace = outputPrevious
				End If
			End Try

			Return outputs
		End Function

		Private Function reshapeTimeStepInput(ByVal input As INDArray) As INDArray
			If input.rank() = 2 Then ' dynamically reshape to 3D input with one time-step.
				Dim inShape() As Long = input.shape()
				input = input.reshape(ChrW(inShape(0)), inShape(1), 1)
			End If
			Return input
		End Function


		''' <summary>
		''' Calculate the gradient of the network with respect to some external errors.
		''' Note that this is typically used for things like reinforcement learning, not typical networks that include
		''' an OutputLayer or RnnOutputLayer
		''' </summary>
		''' <param name="epsilons"> Epsilons (errors) at the output. Same order with which the output layers are defined in configuration setOutputs(String...) </param>
		''' <returns> Gradient for the network </returns>
		Public Overridable Function backpropGradient(ParamArray ByVal epsilons() As INDArray) As Gradient
			If epsilons Is Nothing OrElse epsilons.Length <> numOutputArrays_Conflict Then
				Throw New System.ArgumentException("Invalid input: must have epsilons length equal to number of output arrays")
			End If


			Try
				calcBackpropGradients(True, configuration_Conflict.getBackpropType() = BackpropType.TruncatedBPTT, epsilons)
				Return gradient_Conflict
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Do backprop (gradient calculation)
		''' </summary>
		''' <param name="truncatedBPTT">    false: normal backprop. true: calculate gradients using truncated BPTT for RNN layers </param>
		''' <param name="externalEpsilons"> null usually (for typical supervised learning). If not null (and length > 0) then assume that
		'''                         the user has provided some errors externally, as they would do for example in reinforcement
		'''                         learning situations. </param>
		Protected Friend Overridable Sub calcBackpropGradients(ByVal clearLayers As Boolean, ByVal truncatedBPTT As Boolean, ParamArray ByVal externalEpsilons() As INDArray)
			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If

	'        
	'         Design for workspaces use in backprop for ComputationGraph is similar to MultiLayerNetwork and shares some
	'         features with outputOfLayersDetached
	'
	'         Specifically:
	'         1. We assume forward pass has already been done, and hence layer input fields are set (with all arrays/activations in
	'            workspace WS_ALL_LAYERS_ACT if appropriate)
	'         2. We use a set of small workspaces to contain the activation gradients for a single layer
	'            These are opened once per layer, and are closed only once the corresponding activation gradients have been
	'            consumed by all layers
	'         

			If externalEpsilons Is Nothing OrElse externalEpsilons.Length = 0 AndAlso configuration_Conflict.getTrainingWorkspaceMode() <> WorkspaceMode.NONE Then
				WorkspaceUtils.assertOpenAndActive(WS_ALL_LAYERS_ACT, "Expected workspace WS_ALL_LAYERS_ACT to be active and open" & " in calcBackpropGradients when workspace mode is not set to NONE")
			End If

			'Validate the network configuration for external errors - no output layers
			If externalEpsilons IsNot Nothing AndAlso externalEpsilons.Length > 0 Then
				Dim outputLayers As IList(Of String) = configuration_Conflict.getNetworkOutputs()
				For Each s As String In outputLayers
					Dim gv As GraphVertex = getVertex(s)
					If TypeOf gv Is LayerVertex AndAlso TypeOf (DirectCast(gv, LayerVertex)).Layer Is IOutputLayer Then
						Throw New System.InvalidOperationException("Cannot perform backprop with external errors in conjunction with an output layer:" & " output layers cannot use external errors for backprop. Layer name: " & s)
					End If
				Next s

			End If

			'Position in array: index of vertex. Value at position: the step (in topological order) that the activation
			' gradients of the specified vertex have been consumed by
			'Put another way: this is the step that it's safe to deallocate the layer's activation gradients by closing the
			' corresponding workspace
			'TODO we can probably cache this...
			Dim vertexActGradsFullyConsumedByStep(topologicalOrder.Length - 1) As Integer
			For Each gv As GraphVertex In vertices_Conflict
				Dim idx As Integer = gv.VertexIndex
				Dim minStepOfInputFrom As Integer = Integer.MaxValue
				Dim inputsFrom() As VertexIndices = gv.InputVertices
				If inputsFrom IsNot Nothing Then
					'inputsFrom may be null for input vertex
					For Each vi As VertexIndices In inputsFrom
						Dim posInTopoSort As Integer = ArrayUtils.IndexOf(topologicalOrder, vi.VertexIndex)
						If posInTopoSort = -1 Then
							Throw New System.InvalidOperationException("Did not find vertex " & vi.VertexIndex & " in topological sort array")
						End If
						minStepOfInputFrom = Math.Min(minStepOfInputFrom, posInTopoSort)
					Next vi
				End If

				If minStepOfInputFrom = Integer.MaxValue Then
					'Input vertex, etc
					vertexActGradsFullyConsumedByStep(idx) = 0
				Else
					vertexActGradsFullyConsumedByStep(idx) = minStepOfInputFrom
				End If
			Next gv


			Dim noWS As Boolean = configuration_Conflict.getInferenceWorkspaceMode() = WorkspaceMode.NONE
			Dim allNone As LayerWorkspaceMgr = If(noWS, LayerWorkspaceMgr.noWorkspaces(helperWorkspaces), Nothing)

			Dim allWorkspaceManagers As IList(Of LayerWorkspaceMgr) = New List(Of LayerWorkspaceMgr)()
			Dim freeWorkspaceManagers As IList(Of LayerWorkspaceMgr) = New List(Of LayerWorkspaceMgr)() 'Basically used as a stack
			Dim openActivationsWorkspaces As IDictionary(Of MemoryWorkspace, LayerWorkspaceMgr) = New IdentityHashMap(Of MemoryWorkspace, LayerWorkspaceMgr)()
			Dim closeAtEndIteraton() As IList(Of MemoryWorkspace) = CType(New System.Collections.IList(topologicalOrder.Length - 1){}, IList(Of MemoryWorkspace)())

			'Do backprop, in reverse topological order
			Dim gradients As New LinkedList(Of Triple(Of String, INDArray, Char))()
			Dim setVertexEpsilon(topologicalOrder.Length - 1) As Boolean 'If true: already set epsilon for this vertex; later epsilons should be *added* to the existing one, not set
			Dim initialWorkspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			Dim traceLog As Boolean = log.isTraceEnabled()

			Dim t As Exception = Nothing
			Try
				For i As Integer = topologicalOrder.Length - 1 To 0 Step -1
					Dim hitFrozen As Boolean = False
					Dim current As GraphVertex = vertices_Conflict(topologicalOrder(i))
					Dim vIdx As Integer = current.VertexIndex
					Dim vertexName As String = current.VertexName

					If traceLog Then
						log.trace("About backprop: {} (""{}"") - {}", i, vertexName, current.GetType().Name)
					End If

					'FIXME: make the frozen vertex feature extraction more flexible
					If current.hasLayer() AndAlso TypeOf current.Layer Is FrozenLayer OrElse TypeOf current Is FrozenVertex Then
						hitFrozen = True
					End If

					If current.InputVertex OrElse hitFrozen Then
						'Close any activation gradient workspaces that we no longer require
						'Note that activation gradient workspaces can be closed only once the corresponding activations
						' gradients have been fully consumed
						If closeAtEndIteraton(i) IsNot Nothing Then
							For Each wsAct As MemoryWorkspace In closeAtEndIteraton(i)
								wsAct.close()
								Dim canNowReuse As LayerWorkspaceMgr = openActivationsWorkspaces.Remove(wsAct)
								freeWorkspaceManagers.Add(canNowReuse)
							Next wsAct
						End If
						closeAtEndIteraton(i) = Nothing
						Continue For
					End If


					'First: determine what workspace manager we should use for the activation gradients from this vertex
					Dim workspaceMgr As LayerWorkspaceMgr
					If noWS Then
						workspaceMgr = allNone
					Else
						'First: is there a free activation gradient workspace we can use?
						If freeWorkspaceManagers.Count > 0 Then
							workspaceMgr = freeWorkspaceManagers.RemoveAt(freeWorkspaceManagers.Count - 1)
						Else
							'No existing free workspace managers for forward pass - create a new one...
							Dim wsName As String = "WS_LAYER_ACT_" & allWorkspaceManagers.Count
							workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATION_GRAD, wsName, WS_LAYER_ACT_X_CONFIG).with(ArrayType.ACTIVATIONS, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

							allWorkspaceManagers.Add(workspaceMgr)
						End If
					End If
					workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)

					If current.OutputVertex Then
						'Two reasons for a vertex to be an output vertex:
						'(a) it's an output layer (i.e., instanceof IOutputLayer), or
						'(b) it's a normal layer, but it has been marked as an output layer for use in external errors - for reinforcement learning, for example

						Dim thisOutputNumber As Integer = configuration_Conflict.getNetworkOutputs().IndexOf(current.VertexName)
						Dim currentLayer As Layer = current.Layer
						If TypeOf currentLayer Is FrozenLayerWithBackprop Then
							currentLayer = DirectCast(currentLayer, FrozenLayerWithBackprop).InsideLayer
						End If
						If TypeOf currentLayer Is IOutputLayer Then
							Dim outputLayer As IOutputLayer = DirectCast(currentLayer, IOutputLayer)

							Dim currLabels As INDArray = labels_Conflict(thisOutputNumber)
							outputLayer.Labels = currLabels
						Else
							If (externalEpsilons Is Nothing OrElse externalEpsilons.Length = 0) AndAlso labels_Conflict(thisOutputNumber) IsNot Nothing Then
								Throw New DL4JException("Layer """ & current.VertexName & """ of type " & current.Layer.GetType().Name & " is set as network output " & "(but isn't an IOutputLayer). Only IOutputLayer layers can be fit via backprop with" & " a labels array. ")
							End If
							current.Epsilon = externalEpsilons(thisOutputNumber)
							setVertexEpsilon(topologicalOrder(i)) = True
						End If
					End If

					'Actually execute backprop for the specified vertex
					'First: Open the relevant workspace for the activations.
					'Note that this will be closed only once the current vertex's activations have been consumed
					Dim wsActivationGrads As MemoryWorkspace = workspaceMgr.notifyScopeEntered(ArrayType.ACTIVATION_GRAD)
					openActivationsWorkspaces(wsActivationGrads) = workspaceMgr

					'Note that because we're opening activation gradient workspaces not in any defined order (i.e., workspace
					' use isn't simply nested), we'll manually override the previous workspace setting. Otherwise, when we
					' close these workspaces, the "current" workspace may be set to the incorrect one
					wsActivationGrads.PreviousWorkspace = initialWorkspace

					Dim closeableAt As Integer = vertexActGradsFullyConsumedByStep(vIdx)
					If closeableAt >= 0 Then
						If closeAtEndIteraton(closeableAt) Is Nothing Then
							closeAtEndIteraton(closeableAt) = New List(Of MemoryWorkspace)()
						End If
						closeAtEndIteraton(closeableAt).Add(wsActivationGrads)
					End If

					Dim pair As Pair(Of Gradient, INDArray())
					Dim epsilons() As INDArray
					Using wsWorkingMem As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.BP_WORKING_MEM)
						pair = current.doBackward(truncatedBPTT, workspaceMgr)
						epsilons = pair.Second

						'Validate workspace location for the activation gradients:
						'validateArrayWorkspaces(LayerWorkspaceMgr mgr, INDArray array, ArrayType arrayType, String vertexName, boolean isInputVertex, String op){
						For Each epsilon As INDArray In epsilons
							If epsilon IsNot Nothing Then
								'May be null for EmbeddingLayer, etc
								validateArrayWorkspaces(workspaceMgr, epsilon, ArrayType.ACTIVATION_GRAD, vertexName, False, "Backprop")
							End If
						Next epsilon
					End Using

					'Inputs to the current GraphVertex:
					Dim inputVertices() As VertexIndices = current.InputVertices

					'Set epsilons for the vertices that provide inputs to this vertex:
					If inputVertices IsNot Nothing Then
						Dim j As Integer = 0
						For Each v As VertexIndices In inputVertices
							Dim gv As GraphVertex = vertices_Conflict(v.VertexIndex)
							If setVertexEpsilon(gv.VertexIndex) Then
								'This vertex: must output to multiple vertices... we want to add the epsilons here
								Dim currentEps As INDArray = gv.Epsilon
								If currentEps Is Nothing Then
									'Edge case: this can be null for dual embedding layer case - in -> e1, in -> e2
									gv.Epsilon = currentEps
								Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: gv.setEpsilon(currentEps.addi(epsilons[j++]));
									gv.Epsilon = currentEps.addi(epsilons(j)) 'TODO is this always safe?
										j += 1
								End If
							Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: gv.setEpsilon(epsilons[j++]);
								gv.Epsilon = epsilons(j)
									j += 1
							End If
							setVertexEpsilon(gv.VertexIndex) = True
						Next v
					End If

					If pair.First IsNot Nothing Then
						Dim g As Gradient = pair.First
						Dim map As IDictionary(Of String, INDArray) = g.gradientForVariable()
						Dim tempList As New LinkedList(Of Triple(Of String, INDArray, Char))()
						For Each entry As KeyValuePair(Of String, INDArray) In map.SetOfKeyValuePairs()
							Dim origName As String = entry.Key
							Dim newName As String = current.VertexName & "_" & origName
							tempList.AddFirst(New Triple(Of )(newName, entry.Value, g.flatteningOrderForVariable(origName)))
						Next entry
						For Each triple As Triple(Of String, INDArray, Char) In tempList
							gradients.AddFirst(triple)
						Next triple
					End If

					'Close any activation gradient workspaces that we no longer require
					'Note that activation gradient workspaces can be closed only once the corresponding activations
					' gradients have been fully consumed
					If closeAtEndIteraton(i) IsNot Nothing Then
						For Each wsAct As MemoryWorkspace In closeAtEndIteraton(i)
							wsAct.close()
							Dim canNowReuse As LayerWorkspaceMgr = openActivationsWorkspaces.Remove(wsAct)
							freeWorkspaceManagers.Add(canNowReuse)
						Next wsAct
						closeAtEndIteraton(i) = Nothing
					End If

					If traceLog Then
						log.trace("Completed backprop: {} (""{}"") - {}", i, vertexName, current.GetType().Name)
					End If
				Next i
			Catch t2 As Exception
				t = t2
			Finally
				'Close all open workspaces... usually this list will be empty, but not if an exception is thrown
				For Each ws As MemoryWorkspace In openActivationsWorkspaces.Keys
					Try
						ws.close()
					Catch t2 As Exception
						If t IsNot Nothing Then
							log.error("Encountered second exception while trying to close workspace after initial exception")
							log.error("Original exception:", t)
							Throw t2
						End If
					End Try
				Next ws
				Nd4j.MemoryManager.CurrentWorkspace = initialWorkspace

				If t IsNot Nothing Then
					If TypeOf t Is Exception Then
						Throw (CType(t, Exception))
					End If
					Throw New Exception("Error during neural network backpropagation calculation", t)
				End If
			End Try

			'Now, add the gradients in the order we need them in for flattening (same as params order)
			Dim gradient As Gradient = New DefaultGradient(flattenedGradients)
			For Each tr As Triple(Of String, INDArray, Char) In gradients
				gradient.setGradientFor(tr.getFirst(), tr.getSecond(), tr.getThird())
			Next tr

			Me.gradient_Conflict = gradient

			If truncatedBPTT AndAlso clearTbpttState Then
				rnnClearPreviousState()
			End If

			'Clear inputs and epsilons:
			If clearLayers Then
				For Each gv As GraphVertex In vertices_Conflict
					gv.clear()
				Next gv
			End If
		End Sub

		Public Overrides Function clone() As ComputationGraph
			Dim cg As New ComputationGraph(configuration_Conflict.clone())
			cg.init(params().dup(), False)
			If solver IsNot Nothing Then
				'If  solver is null: updater hasn't been initialized -> getUpdater call will force initialization, however
				Dim u As ComputationGraphUpdater = Me.Updater
				Dim updaterState As INDArray = u.StateViewArray
				If updaterState IsNot Nothing Then
					cg.Updater.StateViewArray = updaterState.dup()
				End If
			End If
			cg.trainingListeners = Me.trainingListeners
			For i As Integer = 0 To topologicalOrder.Length - 1
				If Not vertices_Conflict(topologicalOrder(i)).hasLayer() Then
					Continue For
				End If
				Dim layerName As String = vertices_Conflict(topologicalOrder(i)).VertexName
				If TypeOf getLayer(layerName) Is FrozenLayer Then
					cg.getVertex(layerName).setLayerAsFrozen()
				End If
			Next i
			Return cg
		End Function


		Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Dim scoreSum As Double = 0.0
			For i As Integer = 0 To layers_Conflict.Length - 1
				scoreSum += layers_Conflict(i).calcRegularizationScore(backpropParamsOnly)
			Next i
			Return scoreSum
		End Function

		''' <summary>
		''' Set the trainingListeners for the ComputationGraph (and all layers in the network)
		''' </summary>
		Public Overridable Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				If layers_Conflict Is Nothing Then
					init()
				End If
    
				For Each l As Layer In layers_Conflict
					l.setListeners(listeners)
				Next l
    
				If solver IsNot Nothing Then
					solver.Listeners = listeners
				End If
    
				Me.trainingListeners.Clear()
				If listeners IsNot Nothing Then
					Me.trainingListeners.addAll(listeners)
				End If
			End Set
			Get
				Return trainingListeners
			End Get
		End Property

		''' <summary>
		''' Set the trainingListeners for the ComputationGraph (and all layers in the network)
		''' </summary>
		Public Overridable WriteOnly Property Listeners Implements Model.setListeners As TrainingListener()
			Set(ByVal listeners() As TrainingListener)
				Dim list As IList(Of TrainingListener) = New List(Of TrainingListener)()
				'Check: user might have done setListeners(null) thinking this would clear the current listeners.
				'This results in an TrainingListener[1] with a single null value -> results in a NPE later
				If listeners IsNot Nothing AndAlso listeners.Length > 0 Then
					For Each i As TrainingListener In listeners
						If i IsNot Nothing Then
							list.Add(i)
						End If
					Next i
				End If
				setListeners(list)
			End Set
		End Property

		''' <summary>
		''' This method ADDS additional TrainingListener to existing listeners
		''' </summary>
		''' <param name="listeners"> Listeners to add </param>
		Public Overridable Sub addListeners(ParamArray ByVal listeners() As TrainingListener) Implements Model.addListeners
			If Me.trainingListeners Is Nothing Then
				setListeners(listeners)
				Return
			Else
				Dim newListeners As IList(Of TrainingListener) = New List(Of TrainingListener)(Me.trainingListeners) 'To avoid immutable list issues
				Collections.addAll(newListeners, listeners)
				setListeners(newListeners)
			End If

			If solver IsNot Nothing Then
				solver.Listeners = Me.trainingListeners
			End If
		End Sub


		''' <summary>
		''' Get the ComputationGraphUpdater for the network. Creates one on demand, if required
		''' </summary>
		Public Overridable Property Updater As ComputationGraphUpdater
			Get
				Return getUpdater(True)
			End Get
			Set(ByVal updater As ComputationGraphUpdater)
				If solver Is Nothing Then
					solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
				End If
				solver.Optimizer.UpdaterComputationGraph = updater
			End Set
		End Property

		''' <summary>
		''' Get the ComputationGraphUpdater for this network </summary>
		''' <param name="initializeIfAbsent"> If true: create the updater if one is absent. False: return null if absent. </param>
		''' <returns> Updater </returns>
		Public Overridable Function getUpdater(ByVal initializeIfAbsent As Boolean) As ComputationGraphUpdater
			If solver Is Nothing AndAlso initializeIfAbsent Then
				solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
				solver.Optimizer.UpdaterComputationGraph = New ComputationGraphUpdater(Me)
			End If
			If solver IsNot Nothing Then
				Return solver.Optimizer.getComputationGraphUpdater(initializeIfAbsent)
			End If
			Return Nothing
		End Function


		''' <summary>
		''' Get the specified output layer, by index. The index of the output
		''' layer may be 0 to <seealso cref="getNumOutputArrays()"/>-1
		''' </summary>
		Public Overridable Function getOutputLayer(ByVal outputLayerIdx As Integer) As Layer
			If outputLayerIdx >= numOutputArrays_Conflict Then
				Throw New System.ArgumentException("Invalid index: cannot get output layer " & outputLayerIdx & ", total number of network outputs = " & numOutputArrays_Conflict)
			End If
			Return getLayer(configuration_Conflict.getNetworkOutputs().get(outputLayerIdx))
		End Function

		''' @deprecated To be removed. Use <seealso cref="params()"/> 
		<Obsolete("To be removed. Use <seealso cref=""params()""/>")>
		Public Overridable Function params(ByVal backwardOnly As Boolean) As INDArray
			Return params()
		End Function

		''' <summary>
		''' Sets the input and labels and returns a score for the prediction with respect to the true labels<br>
		''' This is equivalent to <seealso cref="score(DataSet, Boolean)"/> with training==true.<br>
		''' <b>NOTE:</b> this version of the score function can only be used with ComputationGraph networks that have
		''' a single input and a single output.
		''' </summary>
		''' <param name="dataSet"> the data to score </param>
		''' <returns> the score for the given input,label pairs </returns>
		''' <seealso cref= #score(DataSet, boolean) </seealso>
		Public Overridable Function score(ByVal dataSet As DataSet) As Double
			Return score(dataSet, False)
		End Function

		''' <summary>
		''' Sets the input and labels and returns a score for the prediction with respect to the true labels<br>
		''' <b>NOTE:</b> this version of the score function can only be used with ComputationGraph networks that have
		''' a single input and a single output. Use <seealso cref="score(MultiDataSet, Boolean)"/> for multiple input/output networks
		''' </summary>
		''' <param name="dataSet">  the data to score </param>
		''' <param name="training"> whether score is being calculated at training time (true) or test time (false) </param>
		''' <returns> the score for the given input,label pairs </returns>
		''' <seealso cref= #score(DataSet, boolean) </seealso>
		Public Overridable Function score(ByVal dataSet As DataSet, ByVal training As Boolean) As Double
			If numInputArrays_Conflict <> 1 OrElse numOutputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot score ComputationGraph network with " & " DataSet: network does not have 1 input and 1 output arrays")
			End If
			Return score(ComputationGraphUtil.toMultiDataSet(dataSet), training)
		End Function

		''' <summary>
		''' Score the network given the MultiDataSet, at test time
		''' </summary>
		Public Overridable Function score(ByVal dataSet As MultiDataSet) As Double
			Return score(dataSet, False)
		End Function

		''' <summary>
		''' Sets the input and labels and returns a score for the prediction with respect to the true labels<br>
		''' </summary>
		''' <param name="dataSet">  the data to score </param>
		''' <param name="training"> whether score is being calculated at training time (true) or test time (false) </param>
		''' <returns> the score for the given input,label pairs </returns>
		Public Overridable Function score(ByVal dataSet As MultiDataSet, ByVal training As Boolean) As Double
			Try
				Return scoreHelper(dataSet, training)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Private Function scoreHelper(ByVal dataSet As MultiDataSet, ByVal training As Boolean) As Double
			Dim mgr As LayerWorkspaceMgr
			Dim wsm As WorkspaceMode = (If(training, configuration_Conflict.getTrainingWorkspaceMode(), configuration_Conflict.getInferenceWorkspaceMode()))
			If wsm = WorkspaceMode.NONE Then
				mgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				mgr = LayerWorkspaceMgr.builder().noWorkspaceFor(ArrayType.ACTIVATIONS).noWorkspaceFor(ArrayType.INPUT).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
			End If
			mgr.setHelperWorkspacePointers(helperWorkspaces)

			Dim hasMaskArrays As Boolean = dataSet.hasMaskArrays()
			If hasMaskArrays Then
				setLayerMaskArrays(dataSet.FeaturesMaskArrays, dataSet.LabelsMaskArrays)
			End If

			Dim score As Double = 0.0
			Inputs = dataSet.Features
			'TODO Can possibly optimize this, in terms of memory use/workspaces
			ffToLayerActivationsDetached(training, FwdPassType.STANDARD, False, vertices_Conflict.Length-1, OutputLayerIndices, dataSet.Features, dataSet.FeaturesMaskArrays,dataSet.LabelsMaskArrays, False)

			'Need to feed forward, but not the output layers
			Using ws As org.nd4j.linalg.workspace.WorkspacesCloseable = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS,org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM,org.deeplearning4j.nn.workspace.ArrayType.RNN_FF_LOOP_WORKING_MEM)

				Dim labels() As INDArray = dataSet.Labels
				Me.Labels = labels

				'Score: sum of the scores for the various output layers...
				Dim r As Double = calcRegularizationScore(True)

				Dim i As Integer = 0
				For Each s As String In configuration_Conflict.getNetworkOutputs()
					Dim gv As GraphVertex = verticesMap(s)
					Dim outLayer As Layer = gv.Layer
					If outLayer Is Nothing OrElse Not (TypeOf outLayer Is IOutputLayer) Then
						log.warn("Cannot calculate score: vertex """ & s & """ is not an output layer")
						Return 0.0
					End If

					Dim ol As IOutputLayer = DirectCast(outLayer, IOutputLayer)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ol.setLabels(labels[i++]);
					ol.Labels = labels(i)
						i += 1

					score += DirectCast(gv, LayerVertex).computeScore(r, training, mgr)

					'Only want to add l1/l2 once...
					r = 0.0
				Next s
			End Using

			clearLayersStates() 'Clean up layer inputs/mask arrays - may be invalidated by workspace
			Return score
		End Function

		''' <summary>
		''' Calculate the score for each example in a DataSet individually. Unlike <seealso cref="score(DataSet)"/> and <seealso cref="score(DataSet, Boolean)"/>
		''' this method does not average/sum over examples. This method allows for examples to be scored individually (at test time only), which
		''' may be useful for example for autoencoder architectures and the like.<br>
		''' Each row of the output (assuming addRegularizationTerms == true) is equivalent to calling score(DataSet) with a single example.
		''' </summary>
		''' <param name="data">                   The data to score </param>
		''' <param name="addRegularizationTerms"> If true: add l1/l2 regularization terms (if any) to the score. If false: don't add regularization terms </param>
		''' <returns> An INDArray (column vector) of size input.numRows(); the ith entry is the score (loss value) of the ith example </returns>
		Public Overridable Function scoreExamples(ByVal data As DataSet, ByVal addRegularizationTerms As Boolean) As INDArray
			If numInputArrays_Conflict <> 1 OrElse numOutputArrays_Conflict <> 1 Then
				Throw New System.NotSupportedException("Cannot score ComputationGraph network with " & " DataSet: network does not have 1 input and 1 output arrays")
			End If
			Return scoreExamples(ComputationGraphUtil.toMultiDataSet(data), addRegularizationTerms)
		End Function

		''' <summary>
		''' Calculate the score for each example in a DataSet individually. Unlike <seealso cref="score(MultiDataSet)"/> and <seealso cref="score(MultiDataSet, Boolean)"/>
		''' this method does not average/sum over examples. This method allows for examples to be scored individually (at test time only), which
		''' may be useful for example for autoencoder architectures and the like.<br>
		''' Each row of the output (assuming addRegularizationTerms == true) is equivalent to calling score(MultiDataSet) with a single example.
		''' </summary>
		''' <param name="dataSet">                The data to score </param>
		''' <param name="addRegularizationTerms"> If true: add l1/l2 regularization terms (if any) to the score. If false: don't add regularization terms </param>
		''' <returns> An INDArray (column vector) of size input.numRows(); the ith entry is the score (loss value) of the ith example </returns>
		Public Overridable Function scoreExamples(ByVal dataSet As MultiDataSet, ByVal addRegularizationTerms As Boolean) As INDArray
			Try
				Return scoreExamplesHelper(dataSet, addRegularizationTerms)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Private Function scoreExamplesHelper(ByVal dataSet As MultiDataSet, ByVal addRegularizationTerms As Boolean) As INDArray
			Dim mgr As LayerWorkspaceMgr
			If configuration_Conflict.getInferenceWorkspaceMode() = WorkspaceMode.NONE Then
				mgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				mgr = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
			End If
			mgr.setHelperWorkspacePointers(helperWorkspaces)

			Dim hasMaskArrays As Boolean = dataSet.hasMaskArrays()
			If hasMaskArrays Then
				setLayerMaskArrays(dataSet.FeaturesMaskArrays, dataSet.LabelsMaskArrays)
			End If

			Dim [out] As INDArray = Nothing
			Inputs = dataSet.Features

			'Need to feed forward, but not the output layers
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				'TODO maybe optimize? We only need *some* of the activations in the WS...
				ffToLayerActivationsInWS(False, vertices_Conflict.Length - 1, OutputLayerIndices, FwdPassType.STANDARD, False, dataSet.Features, dataSet.FeaturesMaskArrays, dataSet.LabelsMaskArrays, False)

				Dim labels() As INDArray = dataSet.Labels
				Me.Labels = labels


				Dim r As Double = (If(addRegularizationTerms, calcRegularizationScore(True), 0.0))
				Dim i As Integer = 0
				For Each s As String In configuration_Conflict.getNetworkOutputs()
					Dim gv As GraphVertex = verticesMap(s)
					Dim outLayer As Layer = gv.Layer
					If outLayer Is Nothing OrElse Not (TypeOf outLayer Is IOutputLayer) Then
						Throw New System.NotSupportedException("Cannot calculate score: vertex """ & s & """ is not an output layer")
					End If

					Dim ol As IOutputLayer = DirectCast(outLayer, IOutputLayer)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: ol.setLabels(labels[i++]);
					ol.Labels = labels(i)
						i += 1

					Dim scoreCurrLayer As INDArray
					Using wsFF As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						scoreCurrLayer =DirectCast(gv, LayerVertex).computeScoreForExamples(r, mgr)
					End Using
					If [out] Is Nothing Then
						[out] = scoreCurrLayer.detach()
					Else
						[out].addi(scoreCurrLayer)
					End If

					'Only want to add l1/l2 once...
					r = 0.0
				Next s
			End Using

			If dataSet.hasMaskArrays() Then
				clearLayerMaskArrays()
			End If
			clearLayersStates()
			Return [out]
		End Function


		'------------------------------------------------------
		'Model methods:

		Public Overridable Sub fit() Implements Model.fit
			fit(inputs_Conflict, labels_Conflict, inputMaskArrays_Conflict, labelMaskArrays_Conflict)
		End Sub

		Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String) Implements Model.update
			Throw New System.NotSupportedException("Not implemented")
		End Sub

		Public Overridable Sub update(ByVal gradient As Gradient) Implements Model.update
			If gradient.gradient().length() <> numParams(True) Then
				Throw New System.ArgumentException("Invalid input: expect gradients array of length " & numParams(True))
			End If
			For Each entry As KeyValuePair(Of String, INDArray) In gradient.gradientForVariable().SetOfKeyValuePairs()
				Dim key As String = entry.Key
				Dim val As INDArray = entry.Value
				Dim idx As Integer = key.LastIndexOf("_"c)
				If idx = -1 Then
					Throw New System.InvalidOperationException("Invalid param key: not have layer separator: """ & key & """")
				End If
				Dim layerName As String = key.Substring(0, idx)
				Dim paramType As String = key.Split("_", True)(1)
				' Update graph gradient
				Me.gradient_Conflict.gradientForVariable()(key) = val
				' Update layer params
				getLayer(layerName).update(val, paramType)
			Next entry
			' Update layerwise gradient view
			BackpropGradientsViewArray = gradient.gradient()
		End Sub

		Private Sub update(ByVal task As Task)
			If Not initDone Then
				initDone = True
				Dim heartbeat As Heartbeat = Heartbeat.Instance
				task = ModelSerializer.taskByModel(Me)
				Dim env As Environment = EnvironmentUtils.buildEnvironment()
				heartbeat.reportEvent([Event].STANDALONE, env, task)
			End If
		End Sub

		Public Overridable Function score() As Double Implements Model.score
			Return score_Conflict
		End Function

		Public Overridable WriteOnly Property Score As Double
			Set(ByVal score As Double)
				Me.score_Conflict = score
			End Set
		End Property

		Public Overridable Function params() As INDArray Implements Model.params, NeuralNetwork.params
			Return flattenedParams
		End Function

		Public Overridable Function updaterState() As INDArray Implements NeuralNetwork.updaterState
			Return If(Updater IsNot Nothing, Updater.getUpdaterStateViewArray(), Nothing)
		End Function

		Public Overridable Function numParams() As Long Implements Model.numParams
			Return numParams(True)
		End Function

		Public Overridable Function numParams(ByVal backwards As Boolean) As Long Implements Model.numParams
			Dim nParams As Integer = 0
			For Each layer As Layer In layers_Conflict
				nParams += layer.numParams(backwards)
			Next layer
			Return nParams
		End Function

		Public Overridable WriteOnly Property Params Implements Model.setParams As INDArray
			Set(ByVal params As INDArray)
				If params Is flattenedParams Then
					Return 'No op
				End If
    
				If Me.flattenedParams IsNot Nothing AndAlso Me.flattenedParams.length() = params.length() Then
					Me.flattenedParams.assign(params)
					Return
				End If
    
				Dim idx As Integer = 0
				For i As Integer = 0 To topologicalOrder.Length - 1
					If Not vertices_Conflict(topologicalOrder(i)).hasLayer() Then
						Continue For
					End If
    
					Dim layer As Layer = vertices_Conflict(topologicalOrder(i)).Layer
					Dim range As Long = layer.numParams()
					If range <= 0 Then
						Continue For 'Some layers: no parameters (subsampling etc)
					End If
					Dim get As INDArray = params.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(idx, range + idx))
					layer.Params = get
					idx += range
				Next i
			End Set
		End Property

		Public Overridable WriteOnly Property ParamsViewArray Implements Model.setParamsViewArray As INDArray
			Set(ByVal gradient As INDArray)
				Throw New System.NotSupportedException("Not supported")
			End Set
		End Property

		Public Overridable ReadOnly Property GradientsViewArray As INDArray Implements Model.getGradientsViewArray
			Get
				Return flattenedGradients
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray Implements Model.setBackpropGradientsViewArray As INDArray
			Set(ByVal gradient As INDArray)
				Dim paramsSoFar As Integer = 0
				For i As Integer = 0 To topologicalOrder.Length - 1
					If Not vertices_Conflict(topologicalOrder(i)).hasLayer() Then
						Continue For
					End If
    
					Dim layer As Layer = vertices_Conflict(topologicalOrder(i)).Layer
					Dim range As Long = layer.numParams()
					If range <= 0 Then
						Continue For 'Some layers: no parameters (subsampling etc)
					End If
					layer.BackpropGradientsViewArray = gradient.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramsSoFar, paramsSoFar + range))
					paramsSoFar += range
				Next i
			End Set
		End Property

		Public Overridable Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements Model.fit
			Throw New System.NotSupportedException("Cannot pretrain ComputationGraph with single INDArray")
		End Sub

		Public Overridable Function gradient() As Gradient Implements Model.gradient
			Return gradient_Conflict
		End Function

		Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double) Implements Model.gradientAndScore
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function

		Public Overridable Function batchSize() As Integer Implements Model.batchSize
			'In 99+% of cases, the input and labels dimension 0 size should be identical
			'The only real exceptions: space to batch, and batch to space layers
			'In those cases, we should base it on the labels size, as this impacts gradient calculation
			Return If(labels_Conflict Is Nothing OrElse labels_Conflict(0) Is Nothing, CInt(inputs_Conflict(0).size(0)), CInt(labels_Conflict(0).size(0)))
		End Function

		Public Overridable Function conf() As NeuralNetConfiguration
			Return defaultConfiguration
		End Function

		Public Overridable WriteOnly Property Conf As NeuralNetConfiguration
			Set(ByVal conf As NeuralNetConfiguration)
				Throw New System.NotSupportedException()
			End Set
		End Property

		Public Overridable Function input() As INDArray Implements Model.input
			If numInputArrays_Conflict = 1 Then
				Return (If(inputs_Conflict IsNot Nothing, inputs_Conflict(0), Nothing))
			Else
				Throw New System.NotSupportedException("Cannot return single input: ComputationGraph  has multiple inputs")
			End If
		End Function

		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer Implements Model.getOptimizer, NeuralNetwork.getOptimizer
			Get
				Return solver.Optimizer
			End Get
		End Property

		Public Overridable Function getParam(ByVal paramName As String) As INDArray Implements Model.getParam
			'        throw new UnsupportedOperationException("Not implemented");
			Dim idx As Integer = paramName.LastIndexOf("_"c)
			If idx = -1 Then
				Throw New System.InvalidOperationException("Invalid param key: not have layer separator: """ & paramName & """")
			End If
			Dim layerName As String = paramName.Substring(0, idx)
			Dim paramType As String = paramName.Substring(idx + 1)
			Return getLayer(layerName).getParam(paramType)

		End Function

		Public Overridable Function paramTable() As IDictionary(Of String, INDArray)
			Return paramTable(False)
		End Function

		Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			'Get all parameters from all layers/vertices
			Dim allParams As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For Each gv As GraphVertex In vertices_Conflict
				Dim paramMap As IDictionary(Of String, INDArray) = gv.paramTable(backpropParamsOnly)
				For Each entry As KeyValuePair(Of String, INDArray) In paramMap.SetOfKeyValuePairs()
					Dim newKey As String = gv.VertexName & "_" & entry.Key
					allParams(newKey) = entry.Value
				Next entry
			Next gv
			Return allParams
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setParamTable(@NonNull Map<String, org.nd4j.linalg.api.ndarray.INDArray> paramTable)
		Public Overridable WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				Dim m As IDictionary(Of String, INDArray) = Me.paramTable()
				Preconditions.checkArgument(paramTable.Keys.Equals(m.Keys), "Cannot set param table: parameter set keys are not equal")
				Dim current As IDictionary(Of String, INDArray) = Me.paramTable()
				'Check shapes before doing partial assigment to avoid leaving net in incorrect state
				For Each s As String In current.Keys
					Dim arrCurrent As INDArray = current(s)
					Dim arrNew As INDArray = Me.paramTable(s)
					Dim shapeCurrent As val = arrCurrent.shape()
					Dim shapeNew As val = arrNew.shape()
					Preconditions.checkState(shapeCurrent.SequenceEqual(shapeNew), "Cannot set parameters: shape array for " & "parameter ""%s"" does not match existing shape: parameter shape = %s, new param shape = %s", s, shapeCurrent, arrNew)
				Next s
    
				For Each s As String In current.Keys
					Dim arrCurrent As INDArray = current(s)
					Dim arrNew As INDArray = Me.paramTable(s)
					arrCurrent.assign(arrNew)
				Next s
			End Set
		End Property

		Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray) Implements Model.setParam
			'        throw new UnsupportedOperationException("Not implemented");
			Dim idx As Integer = key.LastIndexOf("_"c)
			If idx = -1 Then
				Throw New System.InvalidOperationException("Invalid param key: not have layer separator: """ & key & """")
			End If
			Dim layerName As String = key.Substring(0, idx)
			Dim paramType As String = key.Substring(idx + 1)
			getLayer(layerName).setParam(paramType, val)
		End Sub

		Public Overridable Sub clear() Implements Model.clear
			inputs_Conflict = Nothing
			labels_Conflict = Nothing
			inputMaskArrays_Conflict = Nothing
			labelMaskArrays_Conflict = Nothing
		End Sub

		Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer) Implements Model.applyConstraints
			For Each l As Layer In layers_Conflict
				l.applyConstraints(iteration, epoch)
			Next l
		End Sub

		'------------------------------------------------------------------------------
		'RNN-specific functionality

		''' <summary>
		''' If this ComputationGraph contains one or more RNN layers: conduct forward pass (prediction)
		''' but using previous stored state for any RNN layers. The activations for the final step are
		''' also stored in the RNN layers for use next time rnnTimeStep() is called.<br>
		''' This method can be used to generate output one or more steps at a time instead of always having to do
		''' forward pass from t=0. Example uses are for streaming data, and for generating samples from network output
		''' one step at a time (where samples are then fed back into the network as input)<br>
		''' If no previous state is present in RNN layers (i.e., initially or after calling rnnClearPreviousState()),
		''' the default initialization (usually 0) is used.<br>
		''' Supports mini-batch (i.e., multiple predictions/forward pass in parallel) as well as for single examples.<br>
		''' </summary>
		''' <param name="inputs"> Input to network. May be for one or multiple time steps. For single time step:
		'''               input has shape [miniBatchSize,inputSize] or [miniBatchSize,inputSize,1]. miniBatchSize=1 for single example.<br>
		'''               For multiple time steps: [miniBatchSize,inputSize,inputTimeSeriesLength] </param>
		''' <returns> Output activations. If output is RNN layer (such as RnnOutputLayer): if all inputs have shape [miniBatchSize,inputSize]
		''' i.e., is 2d, then outputs have shape [miniBatchSize,outputSize] (i.e., also 2d) instead of [miniBatchSize,outputSize,1].<br>
		''' Otherwise output is 3d [miniBatchSize,outputSize,inputTimeSeriesLength] when using RnnOutputLayer (or unmodified otherwise). </returns>
		Public Overridable Function rnnTimeStep(ParamArray ByVal inputs() As INDArray) As INDArray()
			Return rnnTimeStepHelper(Nothing, inputs)
		End Function

		''' <summary>
		''' See <seealso cref="rnnTimeStep(INDArray...)"/> for details.<br>
		''' If no memory workspace is provided, the output will be detached (not in any workspace).<br>
		''' If a memory workspace is provided, the output activation array (i.e., the INDArray returned by this method)
		''' will be placed in the specified workspace. This workspace must be opened by the user before calling this method -
		''' and the user is responsible for (a) closing this workspace, and (b) ensuring the output array is not used out
		''' of scope (i.e., not used after closing the workspace to which it belongs - as this is likely to cause either
		''' an exception when used, or a crash).
		''' </summary>
		''' <param name="inputs">          Input activations </param>
		''' <param name="outputWorkspace"> Output workspace. May be null </param>
		''' <returns> The output/activations from the network (either detached or in the specified workspace if provided) </returns>
		Public Overridable Function rnnTimeStep(ByVal outputWorkspace As MemoryWorkspace, ParamArray ByVal inputs() As INDArray) As INDArray()
			Try
				Return rnnTimeStepHelper(outputWorkspace, inputs)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Private Function rnnTimeStepHelper(ByVal outputWs As MemoryWorkspace, ParamArray ByVal inputs() As INDArray) As INDArray()
			Dim inputIs2d As Boolean = True
			For Each i As INDArray In inputs
				If i.rank() <> 2 Then
					inputIs2d = False
					Exit For
				End If
			Next i

			Dim outputs() As INDArray = outputOfLayersDetached(False, FwdPassType.RNN_TIMESTEP, OutputLayerIndices, inputs, Nothing, Nothing, True, False, outputWs)

			'As per MultiLayerNetwork.rnnTimeStep(): if inputs are all 2d, then outputs are all 2d
			If inputIs2d Then
				For i As Integer = 0 To outputs.Length - 1
					If outputs(i).rank() = 3 AndAlso outputs(i).size(2) = 1 Then
						'Return 2d output with shape [miniBatchSize,nOut]
						' instead of 3d output with shape [miniBatchSize,nOut,1]
						outputs(i) = outputs(i).tensorAlongDimension(0, 1, 0)
					End If
				Next i
			End If

			Me.inputs_Conflict = Nothing
			Return outputs
		End Function

		''' <summary>
		''' Get the state of the RNN layer, as used in <seealso cref="rnnTimeStep(INDArray...)"/>.
		''' </summary>
		''' <param name="layer"> Number/index of the layer. </param>
		''' <returns> Hidden state, or null if layer is not an RNN layer </returns>
		Public Overridable Function rnnGetPreviousState(ByVal layer As Integer) As IDictionary(Of String, INDArray)
			Return rnnGetPreviousState(layers_Conflict(layer).conf().getLayer().getLayerName())
		End Function

		''' <summary>
		''' Get the state of the RNN layer, as used in <seealso cref="rnnTimeStep(INDArray...)"/>.
		''' </summary>
		''' <param name="layerName"> name of the layer </param>
		''' <returns> Hidden state, or null if layer is not an RNN layer </returns>
		Public Overridable Function rnnGetPreviousState(ByVal layerName As String) As IDictionary(Of String, INDArray)
			Dim l As Layer = verticesMap(layerName).getLayer()
			If TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer Then
				l = DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer).getUnderlying()
			End If
			If l Is Nothing OrElse Not (TypeOf l Is RecurrentLayer) Then
				Return Nothing
			End If
			Return DirectCast(l, RecurrentLayer).rnnGetPreviousState()
		End Function

		''' <summary>
		''' Get a map of states for ALL RNN layers, as used in <seealso cref="rnnTimeStep(INDArray...)"/>.
		''' Layers that are not RNN layers will not have an entry in the returned map
		''' </summary>
		''' <returns> Map of states (keyed by layer name) or null if layer is not an RNN layer </returns>
		''' <seealso cref= #rnnSetPreviousStates(Map) </seealso>
		Public Overridable Function rnnGetPreviousStates() As IDictionary(Of String, IDictionary(Of String, INDArray))
			Dim states As IDictionary(Of String, IDictionary(Of String, INDArray)) = New Dictionary(Of String, IDictionary(Of String, INDArray))()
			For Each l As Layer In layers_Conflict
				If TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer Then
					l = DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer).getUnderlying()
				End If
				If TypeOf l Is RecurrentLayer Then
					states(l.conf().getLayer().getLayerName()) = DirectCast(l, RecurrentLayer).rnnGetPreviousState()
				End If
			Next l
			Return states
		End Function

		''' <summary>
		''' Set the state of the RNN layer, for use in <seealso cref="rnnTimeStep(INDArray...)"/>
		''' </summary>
		''' <param name="layer"> The number/index of the layer. </param>
		''' <param name="state"> The state to set the specified layer to </param>
		Public Overridable Sub rnnSetPreviousState(ByVal layer As Integer, ByVal state As IDictionary(Of String, INDArray))
			rnnSetPreviousState(layers_Conflict(layer).conf().getLayer().getLayerName(), state)
		End Sub

		''' <summary>
		''' Set the state of the RNN layer, for use in <seealso cref="rnnTimeStep(INDArray...)"/>
		''' </summary>
		''' <param name="layerName"> The name of the layer. </param>
		''' <param name="state">     The state to set the specified layer to </param>
		Public Overridable Sub rnnSetPreviousState(ByVal layerName As String, ByVal state As IDictionary(Of String, INDArray))
			Dim l As Layer = verticesMap(layerName).getLayer()
			If TypeOf l Is org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer Then
				l = DirectCast(l, org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer).getUnderlying()
			End If
			If l Is Nothing OrElse Not (TypeOf l Is RecurrentLayer) Then
				Throw New System.NotSupportedException("Layer """ & layerName & """ is not a recurrent layer. Cannot set state")
			End If
			DirectCast(l, RecurrentLayer).rnnSetPreviousState(state)
		End Sub

		''' <summary>
		''' Set the states for all RNN layers, for use in <seealso cref="rnnTimeStep(INDArray...)"/>
		''' </summary>
		''' <param name="previousStates"> The previous time step states for all layers (key: layer name. Value: layer states) </param>
		''' <seealso cref= #rnnGetPreviousStates() </seealso>
		Public Overridable Sub rnnSetPreviousStates(ByVal previousStates As IDictionary(Of String, IDictionary(Of String, INDArray)))
			For Each entry As KeyValuePair(Of String, IDictionary(Of String, INDArray)) In previousStates.SetOfKeyValuePairs()
				rnnSetPreviousState(entry.Key, entry.Value)
			Next entry
		End Sub

		''' <summary>
		''' Clear the previous state of the RNN layers (if any), used in <seealso cref="rnnTimeStep(INDArray...)"/>
		''' </summary>
		Public Overridable Sub rnnClearPreviousState()
			If layers_Conflict Is Nothing Then
				Return
			End If
			For Each layer As Layer In layers_Conflict
				If TypeOf layer Is RecurrentLayer Then
					DirectCast(layer, RecurrentLayer).rnnClearPreviousState()
				ElseIf TypeOf layer Is MultiLayerNetwork Then
					DirectCast(layer, MultiLayerNetwork).rnnClearPreviousState()
				End If
			Next layer
		End Sub

		''' <summary>
		''' Fit the network using truncated BPTT
		''' </summary>
		Protected Friend Overridable Sub doTruncatedBPTT(ByVal inputs() As INDArray, ByVal labels() As INDArray, ByVal featureMasks() As INDArray, ByVal labelMasks() As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If

			'Approach used here to implement truncated BPTT: if input is 3d, split it. Otherwise: input is unmodified
			Dim timeSeriesLength As Long = -1
			For Each [in] As INDArray In inputs
				If [in].rank() <> 3 Then
					Continue For
				End If
				If timeSeriesLength = -1 Then
					timeSeriesLength = [in].size(2)
				ElseIf timeSeriesLength <> [in].size(2) Then
					log.warn("Cannot do TBPTT with time series of different lengths")
					Return
				End If
			Next [in]
			For Each [out] As INDArray In labels
				If [out].rank() <> 3 Then
					Continue For
				End If
				If timeSeriesLength = -1 Then
					timeSeriesLength = [out].size(2)
				ElseIf timeSeriesLength <> [out].size(2) Then
					log.warn("Cannot do TBPTT with time series of different lengths")
					Return
				End If
			Next [out]

			Dim fwdLen As Long = configuration_Conflict.getTbpttFwdLength()
			Dim nSubsets As Long = timeSeriesLength \ fwdLen
			If timeSeriesLength Mod fwdLen <> 0 Then
				nSubsets += 1
			End If

			rnnClearPreviousState()

			For i As Integer = 0 To nSubsets - 1
				Dim startTimeIdx As Long = i * fwdLen
				Dim endTimeIdx As Long = startTimeIdx + fwdLen
				If endTimeIdx > timeSeriesLength Then
					endTimeIdx = timeSeriesLength
				End If

				If startTimeIdx > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				Dim list As IList(Of INDArray()) = getSubsetsForTbptt(CInt(startTimeIdx), endTimeIdx, inputs, labels, featureMasks, labelMasks)

				Me.Inputs = list(0)
				Me.Labels = list(1)
				setLayerMaskArrays(list(2), list(3))

				If solver Is Nothing Then
					Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
					End Using
				End If
				solver.optimize(workspaceMgr)

				'Finally, update the state of the RNN layers:
				rnnUpdateStateWithTBPTTState()
			Next i

			If clearTbpttState Then
				rnnClearPreviousState()
			End If
			clearLayerMaskArrays()
		End Sub

		Private Function getSubsetsForTbptt(ByVal startTimeIdx As Integer, ByVal endTimeIdx As Long, ByVal inputs() As INDArray, ByVal labels() As INDArray, ByVal featureMasks() As INDArray, ByVal labelMasks() As INDArray) As IList(Of INDArray())
			Dim newInputs(inputs.Length - 1) As INDArray
			Dim newLabels(labels.Length - 1) As INDArray
			Dim newFeatureMasks() As INDArray = (If(featureMasks IsNot Nothing, New INDArray(featureMasks.Length - 1){}, Nothing))
			Dim newLabelMasks() As INDArray = (If(labelMasks IsNot Nothing, New INDArray(labelMasks.Length - 1){}, Nothing))

			For j As Integer = 0 To inputs.Length - 1
				If inputs(j).rank() <> 3 Then
					newInputs(j) = inputs(j)
				Else
					newInputs(j) = inputs(j).get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
				End If
			Next j
			For j As Integer = 0 To labels.Length - 1
				If labels(j).rank() <> 3 Then
					newLabels(j) = labels(j)
				Else
					newLabels(j) = labels(j).get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
				End If
			Next j
			If featureMasks IsNot Nothing Then
				For j As Integer = 0 To featureMasks.Length - 1
					If featureMasks(j) Is Nothing Then
						Continue For
					End If
					newFeatureMasks(j) = featureMasks(j).get(NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
				Next j
			End If
			If labelMasks IsNot Nothing Then
				For j As Integer = 0 To labelMasks.Length - 1
					If labelMasks(j) Is Nothing Then
						Continue For
					End If
					newLabelMasks(j) = labelMasks(j).get(NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
				Next j
			End If

			Return New List(Of INDArray()) From {newInputs, newLabels, newFeatureMasks, newLabelMasks}
		End Function

		''' <summary>
		''' Similar to rnnTimeStep and feedForward() methods. Difference here is that this method:<br>
		''' (a) like rnnTimeStep does forward pass using stored state for RNN layers, and<br>
		''' (b) unlike rnnTimeStep does not modify the RNN layer state<br>
		''' Therefore multiple calls to this method with the same input should have the same output.<br>
		''' Typically used during training only. Use rnnTimeStep for prediction/forward pass at test time.
		''' </summary>
		''' <param name="inputs">            Input to network </param>
		''' <param name="training">          Whether training or not </param>
		''' <param name="storeLastForTBPTT"> set to true if used as part of truncated BPTT training </param>
		''' <returns> Activations for each layer (including input, as per feedforward() etc) </returns>
		Public Overridable Function rnnActivateUsingStoredState(ByVal inputs() As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean) As IDictionary(Of String, INDArray)
			Return ffToLayerActivationsDetached(training, FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE, storeLastForTBPTT, vertices_Conflict.Length-1, Nothing, inputs, inputMaskArrays_Conflict, labelMaskArrays_Conflict, True)
		End Function

		''' <summary>
		''' Set the mask arrays for features and labels. Mask arrays are typically used in situations such as one-to-many
		''' and many-to-one learning with recurrent neural networks, as well as for supporting time series of varying lengths
		''' within the same minibatch.<br>
		''' For example, with RNN data sets with input of shape [miniBatchSize,nIn,timeSeriesLength] and outputs of shape
		''' [miniBatchSize,nOut,timeSeriesLength], the features and mask arrays will have shape [miniBatchSize,timeSeriesLength]
		''' and contain values 0 or 1 at each element (to specify whether a given input/example is present - or merely padding -
		''' at a given time step).<br>
		''' <b>NOTE</b>: This method is not usually used directly. Instead, the various feedForward and fit methods handle setting
		''' of masking internally.
		''' </summary>
		''' <param name="featureMaskArrays"> Mask array for features (input) </param>
		''' <param name="labelMaskArrays">   Mask array for labels (output) </param>
		''' <seealso cref= #clearLayerMaskArrays() </seealso>
		Public Overridable Sub setLayerMaskArrays(ByVal featureMaskArrays() As INDArray, ByVal labelMaskArrays() As INDArray)
			Me.clearLayerMaskArrays()
			Me.inputMaskArrays_Conflict = featureMaskArrays
			Me.labelMaskArrays_Conflict = labelMaskArrays

			If featureMaskArrays IsNot Nothing Then
				If featureMaskArrays.Length <> numInputArrays_Conflict Then
					Throw New System.ArgumentException("Invalid number of feature mask arrays")
				End If

				Dim minibatchSize As Long = -1
				For Each i As INDArray In featureMaskArrays
					If i IsNot Nothing Then
						minibatchSize = i.size(0)
					End If
				Next i

				'Here: need to do forward pass through the network according to the topological ordering of the network

				Dim map As IDictionary(Of Integer, Pair(Of INDArray, MaskState)) = New Dictionary(Of Integer, Pair(Of INDArray, MaskState))()
				For i As Integer = 0 To topologicalOrder.Length - 1
					Dim current As GraphVertex = vertices_Conflict(topologicalOrder(i))

					If current.InputVertex Then
						Dim fMask As INDArray = featureMaskArrays(current.VertexIndex)
						map(current.VertexIndex) = New Pair(Of INDArray, MaskState)(fMask, MaskState.Active)
					Else
						Dim inputVertices() As VertexIndices = current.InputVertices

						'Now: work out the mask arrays to feed forward...
						Dim inputMasks() As INDArray = Nothing 'new INDArray[inputVertices.length];
						Dim maskState As MaskState = Nothing
						For j As Integer = 0 To inputVertices.Length - 1
							Dim p As Pair(Of INDArray, MaskState) = map(inputVertices(j).VertexIndex)
							If p IsNot Nothing Then
								If inputMasks Is Nothing Then
									inputMasks = New INDArray(inputVertices.Length - 1){}
								End If
								inputMasks(j) = p.First
								If maskState = Nothing OrElse maskState = MaskState.Passthrough Then
									maskState = p.Second
								End If
							End If
						Next j

						If minibatchSize > Integer.MaxValue Then
							Throw New ND4JArraySizeException()
						End If
						Dim outPair As Pair(Of INDArray, MaskState) = current.feedForwardMaskArrays(inputMasks, maskState, CInt(minibatchSize))
						map(topologicalOrder(i)) = outPair
					End If
				Next i
			End If

			If labelMaskArrays IsNot Nothing Then
				If labelMaskArrays.Length <> numOutputArrays_Conflict Then
					Throw New System.ArgumentException("Invalid number of label mask arrays")
				End If
				For i As Integer = 0 To labelMaskArrays.Length - 1
					If labelMaskArrays(i) Is Nothing Then
						' This output doesn't have a mask, we can skip it.
						Continue For
					End If
					Dim outputName As String = configuration_Conflict.getNetworkOutputs().get(i)
					Dim v As GraphVertex = verticesMap(outputName)
					Dim ol As Layer = v.Layer
					ol.MaskArray = labelMaskArrays(i)
				Next i
			End If
		End Sub

		''' <summary>
		''' Remove the mask arrays from all layers.<br>
		''' See <seealso cref="setLayerMaskArrays(INDArray[], INDArray[])"/> for details on mask arrays.
		''' </summary>
		Public Overridable Sub clearLayerMaskArrays()
			For Each layer As Layer In layers_Conflict
				layer.MaskArray = Nothing
			Next layer
			Me.inputMaskArrays_Conflict = Nothing
			Me.labelMaskArrays_Conflict = Nothing
		End Sub

		''' <summary>
		''' Update the internal state of RNN layers after a truncated BPTT fit call
		''' </summary>
		Protected Friend Overridable Sub rnnUpdateStateWithTBPTTState()
			For i As Integer = 0 To layers_Conflict.Length - 1
				If TypeOf layers_Conflict(i) Is RecurrentLayer Then
					Dim l As RecurrentLayer = (DirectCast(layers_Conflict(i), RecurrentLayer))
					l.rnnSetPreviousState(l.rnnGetTBPTTState())
				ElseIf TypeOf layers_Conflict(i) Is MultiLayerNetwork Then
					DirectCast(layers_Conflict(i), MultiLayerNetwork).updateRnnStateWithTBPTTState()
				End If
			Next i
		End Sub

		''' <summary>
		''' Evaluate the network (classification performance - single output ComputationGraphs only)
		''' </summary>
		''' <param name="iterator"> Iterator to evaluate on </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As DataSetIterator) As T
			Return CType(evaluate(iterator, DirectCast(Nothing, IList(Of String))), T)
		End Function

		''' <summary>
		''' Evaluate the network (classification performance - single output ComputationGraphs only)
		''' </summary>
		''' <param name="iterator"> Iterator to evaluate on </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As MultiDataSetIterator) As T
			Return evaluate(iterator, DirectCast(Nothing, IList(Of String)))
		End Function

		''' <summary>
		''' Evaluate the network on the provided data set (single output ComputationGraphs only). Used for evaluating
		''' the performance of classifiers
		''' </summary>
		''' <param name="iterator"> Data to undertake evaluation on </param>
		''' <returns> Evaluation object, summarizing the results of the evaluation on the provided DataSetIterator </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As DataSetIterator, ByVal labelsList As IList(Of String)) As T
			Return evaluate(iterator, labelsList, 1)
		End Function

		''' <summary>
		''' Evaluate the network on the provided data set (single output ComputationGraphs only). Used for evaluating
		''' the performance of classifiers
		''' </summary>
		''' <param name="iterator"> Data to undertake evaluation on </param>
		''' <returns> Evaluation object, summarizing the results of the evaluation on the provided DataSetIterator </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As MultiDataSetIterator, ByVal labelsList As IList(Of String)) As T
			Return evaluate(iterator, labelsList, 1)
		End Function

		''' <summary>
		''' Evaluate the network (for classification) on the provided data set, with top N accuracy in addition to standard accuracy.
		''' For 'standard' accuracy evaluation only, use topN = 1
		''' </summary>
		''' <param name="iterator">   Iterator (data) to evaluate on </param>
		''' <param name="labelsList"> List of labels. May be null. </param>
		''' <param name="topN">       N value for top N accuracy evaluation </param>
		''' <returns> Evaluation object, summarizing the results of the evaluation on the provided DataSetIterator </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As DataSetIterator, ByVal labelsList As IList(Of String), ByVal topN As Integer) As T
			If labelsList Is Nothing Then
				labelsList = iterator.getLabels()
			End If

			Dim outputLayer As Layer = getOutputLayer(0)
			If Configuration.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(Evaluation))
			End If

			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.Evaluation(labelsList, topN))(0), T)
		End Function

		''' <summary>
		''' Evaluate the network (for classification) on the provided data set, with top N accuracy in addition to standard accuracy.
		''' For 'standard' accuracy evaluation only, use topN = 1
		''' </summary>
		''' <param name="iterator">   Iterator (data) to evaluate on </param>
		''' <param name="labelsList"> List of labels. May be null. </param>
		''' <param name="topN">       N value for top N accuracy evaluation </param>
		''' <returns> Evaluation object, summarizing the results of the evaluation on the provided DataSetIterator </returns>
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As MultiDataSetIterator, ByVal labelsList As IList(Of String), ByVal topN As Integer) As T
			Dim outputLayer As Layer = getOutputLayer(0)
			If Configuration.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(Evaluation))
			End If
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.Evaluation(labelsList, topN))(0), T)
		End Function

		''' <summary>
		''' Evaluate the (single output layer only) network for regression performance
		''' </summary>
		''' <param name="iterator"> Data to evaluate on </param>
		''' <returns> Regression evaluation </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal iterator As DataSetIterator) As T
			Return evaluateRegression(iterator, Nothing)
		End Function

		''' <summary>
		''' Evaluate the (single output layer only) network for regression performance
		''' </summary>
		''' <param name="iterator"> Data to evaluate on </param>
		''' <returns> Regression evaluation </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal iterator As MultiDataSetIterator) As T
			Return evaluateRegression(iterator, Nothing)
		End Function

		''' <summary>
		''' Evaluate the (single output layer only) network for regression performance
		''' </summary>
		''' <param name="iterator">    Data to evaluate on </param>
		''' <param name="columnNames"> Column names for the regression evaluation. May be null. </param>
		''' <returns> Regression evaluation </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal iterator As DataSetIterator, ByVal columnNames As IList(Of String)) As T
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.RegressionEvaluation(columnNames))(0), T)
		End Function

		''' <summary>
		''' Evaluate the (single output layer only) network for regression performance
		''' </summary>
		''' <param name="iterator"> Data to evaluate on </param>
		''' <returns> Regression evaluation </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal iterator As MultiDataSetIterator, ByVal columnNames As IList(Of String)) As T
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.RegressionEvaluation(columnNames))(0), T)
		End Function


		''' @deprecated To be removed - use <seealso cref="evaluateROC(DataSetIterator, Integer)"/> to enforce selection of appropriate ROC/threshold configuration 
		<Obsolete("To be removed - use <seealso cref=""evaluateROC(DataSetIterator, Integer)""/> to enforce selection of appropriate ROC/threshold configuration")>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal iterator As DataSetIterator) As T
			Return evaluateROC(iterator, 0)
		End Function

		''' <summary>
		''' Evaluate the network (must be a binary classifier) on the specified data, using the <seealso cref="ROC"/> class
		''' </summary>
		''' <param name="iterator">          Data to evaluate on </param>
		''' <param name="rocThresholdSteps"> Number of threshold steps to use with <seealso cref="ROC"/> </param>
		''' <returns> ROC evaluation on the given dataset </returns>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal iterator As DataSetIterator, ByVal rocThresholdSteps As Integer) As T
			Dim outputLayer As Layer = getOutputLayer(0)
			If Configuration.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(ROC))
			End If
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.ROC(rocThresholdSteps))(0), T)
		End Function

		''' @deprecated To be removed - use <seealso cref="evaluateROC(DataSetIterator, Integer)"/> to enforce selection of appropriate ROC/threshold configuration 
		<Obsolete("To be removed - use <seealso cref=""evaluateROC(DataSetIterator, Integer)""/> to enforce selection of appropriate ROC/threshold configuration")>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal iterator As MultiDataSetIterator) As T
			Return evaluateROC(iterator, 0)
		End Function

		''' <summary>
		''' Evaluate the network (must be a binary classifier) on the specified data, using the <seealso cref="ROC"/> class
		''' </summary>
		''' <param name="iterator">          Data to evaluate on </param>
		''' <param name="rocThresholdSteps"> Number of threshold steps to use with <seealso cref="ROC"/> </param>
		''' <returns> ROC evaluation on the given dataset </returns>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal iterator As MultiDataSetIterator, ByVal rocThresholdSteps As Integer) As T
			Dim outputLayer As Layer = getOutputLayer(0)
			If Configuration.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(ROC))
			End If
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.ROC(rocThresholdSteps))(0), T)
		End Function

		''' @deprecated To be removed - use <seealso cref="evaluateROCMultiClass(DataSetIterator, Integer)"/> to enforce selection of appropriate ROC/threshold configuration 
		<Obsolete("To be removed - use <seealso cref=""evaluateROCMultiClass(DataSetIterator, Integer)""/> to enforce selection of appropriate ROC/threshold configuration")>
		Public Overridable Function evaluateROCMultiClass(Of T As ROCMultiClass)(ByVal iterator As DataSetIterator) As T
			Return evaluateROCMultiClass(iterator, 0)
		End Function

		''' <summary>
		''' Evaluate the network on the specified data, using the <seealso cref="ROCMultiClass"/> class
		''' </summary>
		''' <param name="iterator">          Data to evaluate on </param>
		''' <param name="rocThresholdSteps"> Number of threshold steps to use with <seealso cref="ROCMultiClass"/> </param>
		''' <returns> Multi-class ROC evaluation on the given dataset </returns>
		Public Overridable Function evaluateROCMultiClass(Of T As ROCMultiClass)(ByVal iterator As DataSetIterator, ByVal rocThresholdSteps As Integer) As T
			Dim outputLayer As Layer = getOutputLayer(0)
			If Configuration.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(ROCMultiClass))
			End If
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.ROCMultiClass(rocThresholdSteps))(0), T)
		End Function

		''' <summary>
		''' Evaluate the network on the specified data, using the <seealso cref="ROCMultiClass"/> class
		''' </summary>
		''' <param name="iterator">          Data to evaluate on </param>
		''' <param name="rocThresholdSteps"> Number of threshold steps to use with <seealso cref="ROCMultiClass"/> </param>
		''' <returns> Multi-class ROC evaluation on the given dataset </returns>
		Public Overridable Function evaluateROCMultiClass(Of T As ROCMultiClass)(ByVal iterator As MultiDataSetIterator, ByVal rocThresholdSteps As Integer) As T
			Dim outputLayer As Layer = getOutputLayer(0)
			If Configuration.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(ROCMultiClass))
			End If
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.ROCMultiClass(rocThresholdSteps))(0), T)
		End Function

		''' <summary>
		''' Perform evaluation on the given data (DataSetIterator) with the given <seealso cref="IEvaluation"/> instance
		''' </summary>
		''' <param name="iterator">   Test data to evaluate on </param>
		''' <param name="evaluations"> IEvaluation instances </param>
		''' @param <T>        Type of the IEvaluation instance </param>
		''' <returns> The input IEvaluation instance, after performing evaluation on the test data </returns>
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal iterator As DataSetIterator, ParamArray ByVal evaluations() As T) As T() Implements NeuralNetwork.doEvaluation
			Return doEvaluation(New MultiDataSetIteratorAdapter(iterator), evaluations)
		End Function

		''' <summary>
		''' Perform evaluation on the given data (MultiDataSetIterator) with the given <seealso cref="IEvaluation"/> instance
		''' </summary>
		''' <param name="iterator">    Test data to evaluate on </param>
		''' <param name="evaluations"> IEvaluation insntance </param>
		''' @param <T>         Type of the IEvaluation instance </param>
		''' <returns> The input IEvaluation instance, after performing evaluation on the test data </returns>
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal iterator As MultiDataSetIterator, ParamArray ByVal evaluations() As T) As T() Implements NeuralNetwork.doEvaluation
			Try
				Return doEvaluationHelper(iterator, evaluations)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Perform evaluation for networks with multiple outputs.
		''' </summary>
		''' <param name="iterator">    Data to evaluate </param>
		''' <param name="evaluations"> Evaluation instances. Key: the network output number (0 to numOutputs-1). Value: the IEvaluation
		'''                    instances to perform evaluation with, for that output only. Note that not every output needs to
		'''                    have an IEvaluation[] defined. </param>
		''' <returns> The same evaluation map, after performing evaluation </returns>
		Public Overridable Function evaluate(Of T As IEvaluation)(ByVal iterator As DataSetIterator, ByVal evaluations As IDictionary(Of Integer, T())) As IDictionary(Of Integer, T())
			Return evaluate(New MultiDataSetIteratorAdapter(iterator), evaluations)
		End Function

		''' <summary>
		''' Perform evaluation for networks with multiple outputs.
		''' </summary>
		''' <param name="iterator">    Data to evaluate </param>
		''' <param name="evaluations"> Evaluation instances. Key: the network output number (0 to numOutputs-1). Value: the IEvaluation
		'''                    instances to perform evaluation with, for that output only. Note that not every output needs to
		'''                    have an IEvaluation[] defined. </param>
		''' <returns> The same evaluation map, after performing evaluation </returns>
		Public Overridable Function evaluate(Of T As IEvaluation)(ByVal iterator As MultiDataSetIterator, ByVal evaluations As IDictionary(Of Integer, T())) As IDictionary(Of Integer, T())
			Try
				Return doEvaluationHelper(iterator, evaluations)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") @SafeVarargs private final <T extends org.nd4j.evaluation.IEvaluation> T[] doEvaluationHelper(org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator iterator, T... evaluations)
		Private Function doEvaluationHelper(Of T As IEvaluation)(ByVal iterator As MultiDataSetIterator, ParamArray ByVal evaluations() As T) As T()
			Dim map As IDictionary(Of Integer, IEvaluation()) = Collections.singletonMap(0, CType(evaluations, IEvaluation()))
			Return CType(doEvaluationHelper(iterator, map)(0), T())
		End Function

		Private Function doEvaluationHelper(Of T As IEvaluation)(ByVal iterator As MultiDataSetIterator, ByVal evaluations As IDictionary(Of Integer, T())) As IDictionary(Of Integer, T())
			If layers_Conflict Is Nothing OrElse Not (TypeOf getOutputLayer(0) Is IOutputLayer) Then
				Throw New System.InvalidOperationException("Cannot evaluate network with no output layer")
			End If

			WorkspaceUtils.assertNoWorkspacesOpen("Expected no external workspaces open at start of evaluation (doEvaluationHelper)")

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If iterator.resetSupported() AndAlso Not iterator.hasNext() Then
				iterator.reset()
			End If

			Dim iter As MultiDataSetIterator = If(iterator.asyncSupported(), New AsyncMultiDataSetIterator(iterator, 2, True), iterator)

			Dim cMode As WorkspaceMode = configuration_Conflict.getTrainingWorkspaceMode()
			configuration_Conflict.setTrainingWorkspaceMode(configuration_Conflict.getInferenceWorkspaceMode())

			Dim useRnnSegments As Boolean = (configuration_Conflict.getBackpropType() = BackpropType.TruncatedBPTT)

			Dim outputWs As MemoryWorkspace
			If Configuration.getInferenceWorkspaceMode() = WorkspaceMode.ENABLED Then
				outputWs = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(WS_ALL_LAYERS_ACT_CONFIG, WS_OUTPUT_MEM)
			Else
				outputWs = New DummyWorkspace()
			End If

			Do While iter.MoveNext()
				Dim [next] As MultiDataSet = iter.Current

				If [next].Features Is Nothing OrElse [next].Labels Is Nothing Then
					Continue Do
				End If

				If Not useRnnSegments Then
					'Standard/non-RNN case

					'Assuming single output here
					Dim features() As INDArray = [next].Features
					Dim featuresMasks() As INDArray = [next].FeaturesMaskArrays
					Dim labels() As INDArray = [next].Labels
					Dim labelMasks() As INDArray = [next].LabelsMaskArrays
					Dim meta As IList(Of Serializable) = [next].getExampleMetaData()

					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = outputWs.notifyScopeEntered()
						Dim [out]() As INDArray = outputOfLayersDetached(False, FwdPassType.STANDARD, OutputLayerIndices, features, featuresMasks, labelMasks, True, False, ws)

						For Each i As Integer? In evaluations.Keys
							Preconditions.checkState(i >= 0 AndAlso i <labels.Length, "Invalid output index: evaluation/output indices must be between 0" & " and numOutputs-1 (%s), got index %s", numOutputArrays_Conflict, CInt(i))
							Dim evalsThisOutput() As IEvaluation = evaluations(i)
							If evalsThisOutput Is Nothing Then
								Continue For
							End If

							Preconditions.checkState(i >= 0 AndAlso i < NumOutputArrays, "Invalid output index: indices for outputs " & "must be between 0 and %s inclusive - found index %s", numOutputArrays_Conflict, CInt(i))
							Dim currOut As INDArray = [out](i)
							Dim currLabel As INDArray = labels(i)

							Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
								For Each evaluation As IEvaluation In evalsThisOutput
									evaluation.eval(currLabel, currOut, [next].getLabelsMaskArray(i), meta)
								Next evaluation
							End Using
						Next i
					End Using


				Else
					rnnClearPreviousState()

					Dim fwdLen As Integer = configuration_Conflict.getTbpttFwdLength()
					Dim tsLength As Long = -1
					Dim nF As Long = [next].Features.Length
					For i As Integer = 0 To nF - 1
						If [next].getFeatures(i).rank() = 3 Then
							tsLength = [next].getFeatures(i).size(2)
						End If
					Next i
					If tsLength < 0 Then
						Throw New System.InvalidOperationException("Invalid configuration: detected TBPTT backprop type without" & " time series features")
					End If

					Dim nSubsets As Long = tsLength \ fwdLen
					If tsLength Mod fwdLen <> 0 Then
						nSubsets += 1 'Example: 100 fwdLen with timeSeriesLength=120 -> want 2 subsets (1 of size 100, 1 of size 20)
					End If
					For i As Integer = 0 To nSubsets - 1
						Dim startTimeIdx As Integer = i * fwdLen
						Dim endTimeIdx As Long = Math.Min(startTimeIdx + fwdLen, tsLength)

						Dim subset As IList(Of INDArray()) = getSubsetsForTbptt(startTimeIdx, endTimeIdx, [next].Features, [next].Labels, [next].FeaturesMaskArrays, [next].LabelsMaskArrays)
						setLayerMaskArrays(subset(2), subset(3))

						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = outputWs.notifyScopeEntered()
							Dim outSub() As INDArray = rnnTimeStep(ws, subset(0))

							For Each idx As Integer? In evaluations.Keys
								Dim evalsThisOutput() As IEvaluation = evaluations(idx)
								If evalsThisOutput Is Nothing Then
									Continue For
								End If

								Dim labelSub As INDArray = (If(subset(1) Is Nothing, Nothing, subset(1)(idx)))
								Dim maskSub As INDArray = If(subset(3) Is Nothing, Nothing, subset(3)(idx))
								Dim currOut As INDArray = outSub(idx)
								Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
									For Each evaluation As IEvaluation In evalsThisOutput
										evaluation.eval(labelSub, currOut, maskSub)
									Next evaluation
								End Using
							Next idx
						End Using
					Next i

					rnnClearPreviousState()
				End If

				'Clear inputs, masks etc. Important to avoid leaking invalidated/out of scope arrays between iterations
				clearLayersStates()
			Loop

			If iterator.asyncSupported() Then
				DirectCast(iter, AsyncMultiDataSetIterator).shutdown()
			End If

			configuration_Conflict.setTrainingWorkspaceMode(cMode)

			Return CType(evaluations, IDictionary(Of Integer, T()))
		End Function

		''' <summary>
		''' String detailing the architecture of the computation graph.
		''' Vertices are printed in a topological sort order.
		''' Columns are Vertex Names with layer/vertex type, nIn, nOut, Total number of parameters and the Shapes of the parameters
		''' And the inputs to the vertex
		''' Will also give information about frozen layers/vertices, if any.
		''' </summary>
		''' <returns> Summary as a string </returns>
		''' <seealso cref= #memoryInfo(int, InputType...) </seealso>
		Public Overridable Function summary() As String
			Return summary(DirectCast(Nothing, InputType()))
		End Function

		''' <summary>
		''' String detailing the architecture of the computation graph.
		''' Will also display activation size when given an input type.
		''' Vertices are printed in a topological sort order.
		''' Columns are Vertex Names with layer/vertex type, nIn, nOut, Total number of parameters and the Shapes of the parameters
		''' And the inputs to the vertex
		''' Will also give information about frozen layers/vertices, if any.
		''' </summary>
		''' <returns> Summary as a string </returns>
		''' <seealso cref= #memoryInfo(int, InputType...) </seealso>
		Public Overridable Function summary(ParamArray ByVal inputTypes() As InputType) As String
			Dim ret As New StringBuilder()
			ret.Append(vbLf)

			Dim frozenParams As Integer = 0
			Dim vertexOutputs As IDictionary(Of String, InputType) = New Dictionary(Of String, InputType)() 'vertex name and output types
			Dim currLayerIdx As Integer = -1

			Dim lines As IList(Of String()) = New List(Of String())()
			If inputTypes Is Nothing Then
				lines.Add(New String(){"VertexName (VertexType)", "nIn,nOut", "TotalParams", "ParamsShape", "Vertex Inputs"})
			Else
				lines.Add(New String(){"VertexName (VertexType)", "nIn,nOut", "TotalParams", "ParamsShape", "Vertex Inputs", "InputShape", "OutputShape"})
			End If
			Dim maxLength(If(inputTypes Is Nothing OrElse inputTypes.Length = 0, 5, 7) - 1) As Integer
			Dim header() As String = lines(0)
			For i As Integer = 0 To header.Length - 1
				maxLength(i) = header(i).Length
			Next i

			If topologicalOrder Is Nothing Then
				Dim indices As GraphIndices = calculateIndices()
				topologicalOrder = indices.getTopologicalSortOrder()
			End If

			For Each currVertexIdx As Integer In topologicalOrder

				Dim currentVertex As GraphVertex = vertices_Conflict(currVertexIdx)
				Dim currentVertexName As String = currentVertex.VertexName

				'String vars for print
				Dim classNameArr() As String = currentVertex.GetType().ToString().Split("\.", True)
				Dim className As String = classNameArr(classNameArr.Length - 1)
				Dim connections As String = "-"
				Dim inShape As String = "-"
				Dim outShape As String = "-"
				Dim paramCount As String = "-"
				Dim [in] As String = "-"
				Dim [out] As String = "-"
				Dim paramShape As String = "-"
				If currentVertex.InputVertex Then
					If inputTypes IsNot Nothing Then
						vertexOutputs(currentVertexName) = inputTypes(configuration_Conflict.getNetworkInputs().IndexOf(currentVertexName)) 'for input vertices the outputs are just the input types (only layer vertices have preprocessing?)
					End If
				Else
					connections = configuration_Conflict.getVertexInputs().get(currentVertexName).ToString()
					Dim inputTypeList As IList(Of InputType) = New List(Of InputType)()
					If currentVertex.hasLayer() Then
						Dim currentLayer As Layer = DirectCast(currentVertex, LayerVertex).Layer
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
						classNameArr = currentLayer.GetType().FullName.Split("\.", True)
						className = classNameArr(classNameArr.Length - 1)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
						paramCount = String.Format("%,d", currentLayer.numParams())
						'layer with params
						If currentLayer.numParams() > 0 Then
							paramShape = ""
							If TypeOf currentLayer Is BidirectionalLayer Then ' Bidirectional layer is not an FFL
								Dim bi As BidirectionalLayer = DirectCast(currentLayer, BidirectionalLayer)
								[in] = (CType(bi.conf().getLayer(), Bidirectional).NIn).ToString()
								[out] = (CType(bi.conf().getLayer(), Bidirectional).NOut).ToString()
							Else
								Try
									[in] = (CType(currentLayer.conf().getLayer(), FeedForwardLayer).getNIn()).ToString()
									[out] = (CType(currentLayer.conf().getLayer(), FeedForwardLayer).getNOut()).ToString()
								Catch e As Exception ' Some layers, like PReLU, are just BaseLayers (but have parameters)
								End Try
							End If
							Dim paraNames As IList(Of String) = currentLayer.conf().variables()
							For Each aP As String In paraNames
								Dim paramS As String = ArrayUtils.toString(currentLayer.paramTable()(aP).shape())
								paramShape &= aP & ":" & paramS & ", "
							Next aP
							paramShape = paramShape.Substring(0, paramShape.LastIndexOf(",", StringComparison.Ordinal)).ToString()
						End If
						'frozen layer
						If TypeOf currentLayer Is FrozenLayer Then
							frozenParams += currentLayer.numParams()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							classNameArr = DirectCast(currentLayer, FrozenLayer).InsideLayer.GetType().FullName.Split("\.", True)
							className = "Frozen " & classNameArr(classNameArr.Length - 1)
						End If

						If inputTypes IsNot Nothing Then
							'get input type
							Dim inputVertexName As String = vertices_Conflict(currentVertex.InputVertices(0).VertexIndex).VertexName
							Dim currentInType As InputType = vertexOutputs(inputVertexName)
							inShape = currentInType.ToString()
							inputTypeList.Add(currentInType)

							Dim layerVertexPreProcesor As InputPreProcessor = CType(configuration_Conflict.getVertices().get(currentVertexName), org.deeplearning4j.nn.conf.graph.LayerVertex).PreProcessor
							If layerVertexPreProcesor IsNot Nothing Then
								inShape &= "-->" & layerVertexPreProcesor.getOutputType(currentInType)
							End If
						End If
						currLayerIdx += 1
					Else
						'get input type
						If inputTypes IsNot Nothing Then
							Dim inputVertices() As VertexIndices = currentVertex.InputVertices
							If inputVertices IsNot Nothing Then
								For i As Integer = 0 To inputVertices.Length - 1
									Dim thisInputVertex As GraphVertex = vertices_Conflict(inputVertices(i).VertexIndex)
									inputTypeList.Add(vertexOutputs(thisInputVertex.VertexName))
								Next i
							End If
						End If
					End If
					If inputTypes IsNot Nothing Then
						Dim currentVertexOutputType As InputType = configuration_Conflict.getVertices().get(currentVertexName).getOutputType(currLayerIdx, CType(inputTypeList, List(Of InputType)).ToArray())
						outShape = currentVertexOutputType.ToString()
						vertexOutputs(currentVertexName) = currentVertexOutputType
					End If
				End If

				'Add on to summary string
				Dim line() As String
				If inputTypes Is Nothing Then
					line = New String(){currentVertexName & " (" & className & ")", [in] & "," & [out], paramCount, paramShape, connections}
				Else
					line = New String(){currentVertexName & " (" & className & ")", [in] & "," & [out], paramCount, paramShape, connections, inShape, outShape}
				End If
				For i As Integer = 0 To line.Length - 1
					maxLength(i) = Math.Max(maxLength(i),If(line(i) Is Nothing, 0, line(i).Length))
				Next i
				lines.Add(line)
			Next currVertexIdx

			Dim sbFormat As New StringBuilder()
			Dim totalLength As Integer = 0
			Dim pos As Integer = 0
			For Each length As Integer In maxLength
				Dim currLength As Integer
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if(pos++ == maxLength.length-1)
				If pos = maxLength.Length-1 Then
						pos += 1
					currLength = length
				Else
						pos += 1
					currLength = length+3
				End If
				sbFormat.Append("%-").Append(currLength).Append("s")
				totalLength += currLength
			Next length
			sbFormat.Append(vbLf)
			Dim format As String = sbFormat.ToString()



			ret.Append(StringUtils.repeat("=", totalLength)).Append(vbLf)

			Dim first As Boolean = True
			For Each line As String() In lines
				Dim formatted As String = String.format(format, CType(line, Object()))
				ret.Append(formatted)
				If first Then
					ret.Append(StringUtils.repeat("=", totalLength)).Append(vbLf)
					first = False
				End If
			Next line

			ret.Append(StringUtils.repeat("-", totalLength)).Append(String.format(vbLf & "%30s %,d", "Total Parameters: ", params().length())).Append(String.format(vbLf & "%30s %,d", "Trainable Parameters: ", params().length() - frozenParams)).Append(String.format(vbLf & "%30s %,d", "Frozen Parameters: ", frozenParams)).Append(vbLf).Append(StringUtils.repeat("=", totalLength)).Append(vbLf)

			Return ret.ToString()
		End Function

		''' <summary>
		''' Generate information regarding memory use for the network, for the given input types and minibatch size.
		''' Note that when using workspaces or CuDNN, the network should be trained for some iterations so that the memory
		''' workspaces have time to initialize. Without this, the memory requirements during training may be underestimated.
		''' 
		''' Note also that this is the same information that is generated during an OOM crash when training or performing
		''' inference.
		''' </summary>
		''' <param name="minibatch">    Minibatch size to estimate memory for </param>
		''' <param name="inputTypes">   Input types to the network </param>
		''' <returns> A String with information about network memory use information </returns>
		Public Overridable Function memoryInfo(ByVal minibatch As Integer, ParamArray ByVal inputTypes() As InputType) As String
			Return CrashReportingUtil.generateMemoryStatus(Me, minibatch, inputTypes)
		End Function

		''' <summary>
		''' This method just makes sure there's no state preserved within layers
		''' </summary>
		Public Overridable Sub clearLayersStates()
			For Each layer As Layer In layers_Conflict
				layer.clear()
				layer.clearNoiseWeightParams()
			Next layer

			For Each vertex As GraphVertex In vertices_Conflict
				vertex.clearVertex()
			Next vertex
		End Sub

		''' <summary>
		''' Increment the epoch count (in the underlying <seealso cref="ComputationGraphConfiguration"/> by 1).
		''' Note that this is done <i>automatically</i> when using iterator-based fitting methods, such as
		''' <seealso cref="fit(DataSetIterator)"/> or <seealso cref="fit(MultiDataSet)"/>. However, when using non-iterator fit methods
		''' (DataSet, MultiDataSet, INDArrays etc), the network has no way to know when one epoch ends and another starts.
		''' In such situations, this method can be used to increment the epoch counter.<br>
		''' Note that the epoch counter is used for situations such as some learning rate schedules, and the like.
		''' 
		''' The current epoch count can be obtained using {@code ComputationGraph.getConfiguration().getEpochCount()}
		''' </summary>
		Public Overridable Sub incrementEpochCount()
			configuration_Conflict.setEpochCount(configuration_Conflict.getEpochCount() + 1)
			synchronizeIterEpochCounts()
		End Sub

		Protected Friend Overridable Sub synchronizeIterEpochCounts()
			'TODO: this is necessrry for some schedules - but the redundant values are a little ugly...
			Dim currIter As Integer = Configuration.getIterationCount()
			Dim currEpoch As Integer = Configuration.getEpochCount()
			For Each l As Layer In layers_Conflict
				l.IterationCount = currIter
				l.EpochCount = currEpoch
			Next l
		End Sub

		''' <summary>
		''' Returns the number of iterations (parameter updates) that the ComputationGraph has done </summary>
		''' <returns> Number of iterations </returns>
		Public Overridable ReadOnly Property IterationCount As Integer
			Get
				Return configuration_Conflict.getIterationCount()
			End Get
		End Property

		''' <summary>
		''' Returns the number of epochs that the ComputationGraph has done.
		''' Note that the epoch count is incremented only when <seealso cref="fit(DataSetIterator)"/>, <seealso cref="fit(MultiDataSetIterator)"/>,
		''' <seealso cref="fit(DataSetIterator, Integer)"/> or <seealso cref="fit(MultiDataSetIterator, Integer)"/> are used.
		''' The epoch count can also be manually incremented using <seealso cref="incrementEpochCount()"/> </summary>
		''' <returns> Number of epochs </returns>
		Public Overridable ReadOnly Property EpochCount As Integer
			Get
				Return configuration_Conflict.getEpochCount()
			End Get
		End Property

		''' <summary>
		''' Save the ComputationGraph to a file. Restore using <seealso cref="load(File, Boolean)"/>.
		''' Note that this saves the updater (i.e., the state array for momentum/Adam/rmsprop etc), which is desirable
		''' if further training will be undertaken.
		''' </summary>
		''' <param name="f"> File to save the network to </param>
		''' <seealso cref= ModelSerializer ModelSerializer for more details (and saving/loading via streams) </seealso>
		''' <seealso cref= #save(File, boolean) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(File f) throws IOException
		Public Overridable Sub save(ByVal f As File)
			save(f, True)
		End Sub

		''' <summary>
		''' Save the ComputationGraph to a file. Restore using <seealso cref="load(File, Boolean)"/>.
		''' </summary>
		''' <param name="f"> File to save the network to </param>
		''' <param name="saveUpdater"> If true: save the updater (i.e., the state array for momentum/Adam/rmsprop etc), which should
		'''                    usually be saved if further training is required </param>
		''' <seealso cref= ModelSerializer ModelSerializer for more details (and saving/loading via streams) </seealso>
		''' <seealso cref= #save(File, boolean) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void save(File f, boolean saveUpdater) throws IOException
		Public Overridable Sub save(ByVal f As File, ByVal saveUpdater As Boolean)
			ModelSerializer.writeModel(Me, f, saveUpdater)
		End Sub

		''' <summary>
		''' Restore a ComputationGraph to a file, saved using <seealso cref="save(File)"/> or <seealso cref="ModelSerializer"/> </summary>
		''' <param name="f"> File to load the network from </param>
		''' <param name="loadUpdater"> If true: load the updater if it is available (i.e., the state array for momentum/Adam/rmsprop
		'''                   etc) - use <i>false</i> if no further training is required, or <i>true</i> if further training
		'''                    will be undertaken </param>
		''' <seealso cref= ModelSerializer ModelSerializer for more details (and saving/loading via streams) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static ComputationGraph load(File f, boolean loadUpdater) throws IOException
		Public Shared Function load(ByVal f As File, ByVal loadUpdater As Boolean) As ComputationGraph
			Return ModelSerializer.restoreComputationGraph(f, loadUpdater)
		End Function

		''' <summary>
		''' Return a copy of the network with the parameters and activations set to use the specified (floating point) data type.
		''' If the existing datatype is the same as the requested dataype, the original network will be returned unchanged.
		''' Only floating point datatypes (DOUBLE, FLOAT, HALF) may be used.
		''' </summary>
		''' <param name="dataType"> Datatype to convert the network to </param>
		''' <returns> The network, set to use the specified datatype for the parameters and activations </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ComputationGraph convertDataType(@NonNull DataType dataType)
		Public Overridable Function convertDataType(ByVal dataType As DataType) As ComputationGraph
			Preconditions.checkState(dataType.isFPType(), "Invalid DataType: %s. Can only convert network to a floating point type", dataType)
			If dataType = params().dataType() Then
				Return Me
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Dim newParams As INDArray = params().castTo(dataType)
				Dim jsonConfig As String = Configuration.toJson()
				Dim newConf As ComputationGraphConfiguration = ComputationGraphConfiguration.fromJson(jsonConfig)
				newConf.setDataType(dataType)
				Dim newNet As New ComputationGraph(newConf)
				newNet.init(newParams, False)

				Dim u As Updater = getUpdater(False)
				If u IsNot Nothing AndAlso u.StateViewArray IsNot Nothing Then
					Dim oldUpdaterState As INDArray = u.StateViewArray
					newNet.getUpdater(True).StateViewArray.assign(oldUpdaterState)
				End If
				Return newNet
			End Using
		End Function

		''' <summary>
		''' Set the learning rate for all layers in the network to the specified value. Note that if any learning rate
		''' schedules are currently present, these will be removed in favor of the new (fixed) learning rate.<br>
		''' <br>
		''' <b>Note</b>: <i>This method not free from a performance point of view</i>: a proper learning rate schedule
		''' should be used in preference to calling this method at every iteration.
		''' </summary>
		''' <param name="newLr"> New learning rate for all layers </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(String, double) </seealso>
		Public Overridable WriteOnly Property LearningRate As Double
			Set(ByVal newLr As Double)
				NetworkUtils.setLearningRate(Me, newLr)
			End Set
		End Property

		''' <summary>
		''' Set the learning rate schedule for all layers in the network to the specified schedule.
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="ComputationGraphConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="ComputationGraphConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="newLr"> New learning rate schedule for all layers </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(String, double) </seealso>
		Public Overridable WriteOnly Property LearningRate As ISchedule
			Set(ByVal newLr As ISchedule)
				NetworkUtils.setLearningRate(Me, newLr)
			End Set
		End Property

		''' <summary>
		''' Set the learning rate for a single layer in the network to the specified value. Note that if any learning rate
		''' schedules are currently present, these will be removed in favor of the new (fixed) learning rate.<br>
		''' <br>
		''' <b>Note</b>: <i>This method not free from a performance point of view</i>: a proper learning rate schedule
		''' should be used in preference to calling this method at every iteration. Note also that
		''' <seealso cref="setLearningRate(Double)"/> should also be used in preference, when all layers need to be set to a new LR
		''' </summary>
		''' <param name="layerName"> Name of the layer to set the LR for </param>
		''' <param name="newLr">     New learning rate for a single layer </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(String, double) </seealso>
		Public Overridable Sub setLearningRate(ByVal layerName As String, ByVal newLr As Double)
			NetworkUtils.setLearningRate(Me, layerName, newLr)
		End Sub

		''' <summary>
		''' Set the learning rate schedule for a single layer in the network to the specified value.<br>
		''' Note also that <seealso cref="setLearningRate(ISchedule)"/> should also be used in preference, when all layers need
		''' to be set to a new LR schedule.<br>
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note also that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="ComputationGraphConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="ComputationGraphConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="layerName"> Name of the layer to set the LR schedule for </param>
		''' <param name="newLr">     New learning rate for a single layer </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(String, double) </seealso>
		Public Overridable Sub setLearningRate(ByVal layerName As String, ByVal newLr As ISchedule)
			NetworkUtils.setLearningRate(Me, layerName, newLr)
		End Sub

		''' <summary>
		''' Get the current learning rate, for the specified layer, from the network.
		''' Note: If the layer has no learning rate (no parameters, or an updater without a learning rate) then null is returned </summary>
		''' <param name="layerName">   Layer name </param>
		''' <returns> Learning rate for the specified layer, or null </returns>
		Public Overridable Function getLearningRate(ByVal layerName As String) As Double?
			Return NetworkUtils.getLearningRate(Me, layerName)
		End Function

		''' <summary>
		''' Return the layer size (number of units) for the specified layer.
		''' Note that the meaning of the "layer size" can depend on the type of layer. For example:<br>
		''' - DenseLayer, OutputLayer, recurrent layers: number of units (nOut configuration option)<br>
		''' - ConvolutionLayer: the channels (number of channels)<br>
		''' - Subsampling layers, global pooling layers, etc: size of 0 is always returned<br>
		''' </summary>
		''' <param name="layer"> Index of the layer to get the size of. Must be in range 0 to nLayers-1 inclusive </param>
		''' <returns> Size of the layer </returns>
		Public Overridable Function layerSize(ByVal layer As Integer) As Long
			If layer < 0 OrElse layer > layers_Conflict.Length Then
				Throw New System.ArgumentException("Invalid layer index: " & layer & ". Layer index must be between 0 and " & (layers_Conflict.Length - 1) & " inclusive")
			End If
			Return layerSize(layers_Conflict(layer).conf().getLayer().getLayerName())
		End Function

		''' <summary>
		''' Return the input size (number of inputs) for the specified layer.<br>
		''' Note that the meaning of the "input size" can depend on the type of layer. For example:<br>
		''' - DenseLayer, OutputLayer, etc: the feature vector size (nIn configuration option)<br>
		''' - Recurrent layers: the feature vector size <i>per time step</i> (nIn configuration option)<br>
		''' - ConvolutionLayer: the channels (number of channels)<br>
		''' - Subsampling layers, global pooling layers, etc: size of 0 is always returned<br>
		''' </summary>
		''' <param name="layer"> Index of the layer to get the size of. Must be in range 0 to nLayers-1 inclusive </param>
		''' <returns> Size of the layer </returns>
		Public Overridable Function layerInputSize(ByVal layer As Integer) As Long
			If layer < 0 OrElse layer > layers_Conflict.Length Then
				Throw New System.ArgumentException("Invalid layer index: " & layer & ". Layer index must be between 0 and " & (layers_Conflict.Length - 1) & " inclusive")
			End If
			Return layerInputSize(layers_Conflict(layer).conf().getLayer().getLayerName())
		End Function

		''' <summary>
		''' Return the layer size (number of units) for the specified layer.<br>
		''' Note that the meaning of the "layer size" can depend on the type of layer. For example:<br>
		''' - DenseLayer, OutputLayer, recurrent layers: number of units (nOut configuration option)<br>
		''' - ConvolutionLayer: the channels (number of channels)<br>
		''' - Subsampling layers, global pooling layers, etc: size of 0 is always returned<br>
		''' </summary>
		''' <param name="layerName"> Name of the layer to get the size of </param>
		''' <returns> Size of the layer </returns>
		Public Overridable Function layerSize(ByVal layerName As String) As Long
			Dim l As Layer = getLayer(layerName)
			If l Is Nothing Then
				Throw New System.ArgumentException("No layer with name """ & layerName & """ exists")
			End If
			Dim conf As org.deeplearning4j.nn.conf.layers.Layer = l.conf().getLayer()
			If conf Is Nothing OrElse Not (TypeOf conf Is FeedForwardLayer) Then
				Return 0
			End If
			Dim ffl As FeedForwardLayer = DirectCast(conf, FeedForwardLayer)

			Return ffl.getNOut()
		End Function

		''' <summary>
		''' Return the input size (number of inputs) for the specified layer.<br>
		''' Note that the meaning of the "input size" can depend on the type of layer. For example:<br>
		''' - DenseLayer, OutputLayer, etc: the feature vector size (nIn configuration option)<br>
		''' - Recurrent layers: the feature vector size <i>per time step</i> (nIn configuration option)<br>
		''' - ConvolutionLayer: the channels (number of channels)<br>
		''' - Subsampling layers, global pooling layers, etc: size of 0 is always returned<br>
		''' </summary>
		''' <param name="layerName"> Name of the layer to get the size of </param>
		''' <returns> Size of the layer </returns>
		Public Overridable Function layerInputSize(ByVal layerName As String) As Long
			Dim l As Layer = getLayer(layerName)
			If l Is Nothing Then
				Throw New System.ArgumentException("No layer with name """ & layerName & """ exists")
			End If
			Dim conf As org.deeplearning4j.nn.conf.layers.Layer = l.conf().getLayer()
			If conf Is Nothing OrElse Not (TypeOf conf Is FeedForwardLayer) Then
				Return 0
			End If
			Dim ffl As FeedForwardLayer = DirectCast(conf, FeedForwardLayer)

			Return ffl.getNIn()
		End Function

		''' <summary>
		''' Indicates whether some other object is "equal to" this one.
		''' <para>
		''' The {@code equals} method implements an equivalence relation
		''' on non-null object references:
		''' <ul>
		''' <li>It is <i>reflexive</i>: for any non-null reference value
		''' {@code x}, {@code x.equals(x)} should return
		''' {@code true}.
		''' <li>It is <i>symmetric</i>: for any non-null reference values
		''' {@code x} and {@code y}, {@code x.equals(y)}
		''' should return {@code true} if and only if
		''' {@code y.equals(x)} returns {@code true}.
		''' <li>It is <i>transitive</i>: for any non-null reference values
		''' {@code x}, {@code y}, and {@code z}, if
		''' {@code x.equals(y)} returns {@code true} and
		''' {@code y.equals(z)} returns {@code true}, then
		''' {@code x.equals(z)} should return {@code true}.
		''' <li>It is <i>consistent</i>: for any non-null reference values
		''' {@code x} and {@code y}, multiple invocations of
		''' {@code x.equals(y)} consistently return {@code true}
		''' or consistently return {@code false}, provided no
		''' information used in {@code equals} comparisons on the
		''' objects is modified.
		''' <li>For any non-null reference value {@code x},
		''' {@code x.equals(null)} should return {@code false}.
		''' </ul>
		''' </para>
		''' <para>
		''' The {@code equals} method for class {@code Object} implements
		''' the most discriminating possible equivalence relation on objects;
		''' that is, for any non-null reference values {@code x} and
		''' {@code y}, this method returns {@code true} if and only
		''' if {@code x} and {@code y} refer to the same object
		''' ({@code x == y} has the value {@code true}).
		''' </para>
		''' <para>
		''' Note that it is generally necessary to override the {@code hashCode}
		''' method whenever this method is overridden, so as to maintain the
		''' general contract for the {@code hashCode} method, which states
		''' that equal objects must have equal hash codes.
		''' 
		''' </para>
		''' </summary>
		''' <param name="obj"> the reference object with which to compare. </param>
		''' <returns> {@code true} if this object is the same as the obj
		''' argument; {@code false} otherwise. </returns>
		''' <seealso cref= #hashCode() </seealso>
		''' <seealso cref= HashMap </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing Then
				Return False
			End If
			If TypeOf obj Is ComputationGraph Then
				Dim network As ComputationGraph = DirectCast(obj, ComputationGraph)
				Dim paramsEquals As Boolean = network.params().Equals(params())
				Dim confEquals As Boolean = Configuration.Equals(network.Configuration)
				Dim updaterEquals As Boolean = Updater.equals(network.Updater)
				Return paramsEquals AndAlso confEquals AndAlso updaterEquals
			End If
			Return False
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void writeObject(ObjectOutputStream oos) throws IOException
		Private Sub writeObject(ByVal oos As ObjectOutputStream)
			ModelSerializer.writeModel(Me, oos, True)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void readObject(ObjectInputStream ois) throws ClassNotFoundException, IOException
		Private Sub readObject(ByVal ois As ObjectInputStream)
			Dim cg As val = ModelSerializer.restoreComputationGraph(ois, True)

			Me.defaultConfiguration = cg.defaultConfiguration.clone()
			Me.configuration_Conflict = cg.configuration.clone()
			Me.init()
			Me.flattenedParams.assign(cg.flattenedParams)

			If cg.getUpdater() IsNot Nothing AndAlso cg.getUpdater(False).getStateViewArray() IsNot Nothing Then
				Me.getUpdater(True).StateViewArray.assign(cg.getUpdater(False).getStateViewArray())
			End If
		End Sub

		''' <summary>
		''' Close the network and deallocate all native memory, including: parameters, gradients, updater memory and workspaces
		''' Note that the network should not be used again for any purpose after it has been closed
		''' </summary>
		Public Overridable Sub close() Implements Model.close
			'Close the INDArray and dealloc
			If flattenedParams.closeable() Then
				flattenedParams.close()
			End If

			If flattenedGradients IsNot Nothing AndAlso flattenedGradients.closeable() Then
				flattenedGradients.close()
			End If

			Dim u As Updater = getUpdater(False)
			If u IsNot Nothing AndAlso u.StateViewArray IsNot Nothing Then
				Dim state As INDArray = u.StateViewArray
				If state.closeable() Then
					state.close()
				End If
			End If

			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
			System.GC.Collect()
		End Sub
	End Class

End Namespace