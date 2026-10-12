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
Imports MultiDataSetWrapperIterator = org.deeplearning4j.datasets.iterator.MultiDataSetWrapperIterator
Imports DL4JException = org.deeplearning4j.exception.DL4JException
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports org.deeplearning4j.nn.api
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports RecurrentLayer = org.deeplearning4j.nn.api.layers.RecurrentLayer
Imports org.deeplearning4j.nn.conf
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.layers.FrozenLayerWithBackprop
Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports BidirectionalLayer = org.deeplearning4j.nn.layers.recurrent.BidirectionalLayer
Imports BaseWrapperLayer = org.deeplearning4j.nn.layers.wrapper.BaseWrapperLayer
Imports UpdaterCreator = org.deeplearning4j.nn.updater.UpdaterCreator
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Solver = org.deeplearning4j.optimize.Solver
Imports ConvexOptimizer = org.deeplearning4j.optimize.api.ConvexOptimizer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports GradientsAccumulator = org.deeplearning4j.optimize.solvers.accumulation.GradientsAccumulator
Imports org.deeplearning4j.util
Imports org.nd4j.adapters
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports ROCMultiClass = org.nd4j.evaluation.classification.ROCMultiClass
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports DummyWorkspace = org.nd4j.linalg.api.memory.abstracts.DummyWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
Imports ND4JWorkspaceException = org.nd4j.linalg.workspace.ND4JWorkspaceException
Imports WorkspaceUtils = org.nd4j.linalg.workspace.WorkspaceUtils
Imports OneTimeLogger = org.nd4j.common.util.OneTimeLogger

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

Namespace org.deeplearning4j.nn.multilayer






'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class MultiLayerNetwork implements Serializable, Classifier, Layer, NeuralNetwork
	<Serializable>
	Public Class MultiLayerNetwork
		Implements Classifier, Layer, NeuralNetwork

		'the hidden neural network layers (including output layer)
'JAVA TO VB CONVERTER NOTE: The field layers was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend layers_Conflict() As Layer
		Protected Friend layerMap As New LinkedHashMap(Of String, Layer)()

		'Current training data: input features and labels
'JAVA TO VB CONVERTER NOTE: The field input was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend input_Conflict, labels_Conflict As INDArray

'JAVA TO VB CONVERTER NOTE: The field initCalled was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend initCalled_Conflict As Boolean = False
		Protected Friend trainingListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()

'JAVA TO VB CONVERTER NOTE: The field defaultConfiguration was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend defaultConfiguration_Conflict As NeuralNetConfiguration
'JAVA TO VB CONVERTER NOTE: The field layerWiseConfigurations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend layerWiseConfigurations_Conflict As MultiLayerConfiguration
'JAVA TO VB CONVERTER NOTE: The field gradient was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend gradient_Conflict As Gradient
'JAVA TO VB CONVERTER NOTE: The field score was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend score_Conflict As Double
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected boolean initDone = false;
		Protected Friend initDone As Boolean = False
		Protected Friend flattenedParams As INDArray 'Params for all layers are a view/subset of this array
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient org.nd4j.linalg.api.ndarray.INDArray flattenedGradients;
		<NonSerialized>
		Protected Friend flattenedGradients As INDArray 'Gradients for all layers are a view/subset of this array

		Protected Friend clearTbpttState As Boolean = True 'Mainly for unit testing (should be enabled otherwise)
'JAVA TO VB CONVERTER NOTE: The field lastEtlTime was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Protected Friend lastEtlTime_Conflict As New ThreadLocal(Of Long)()
'JAVA TO VB CONVERTER NOTE: The field mask was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend mask_Conflict As INDArray

		Protected Friend layerIndex As Integer 'For Layer.get/setIndex()

		<NonSerialized>
		Protected Friend solver As Solver 'Used to call optimizers during backprop
		'Workspaces for CUDNN. Pass to LayerWorkspaceMgr for re-use in cudnn helpers
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected transient Map<String,org.bytedeco.javacpp.Pointer> helperWorkspaces = new HashMap<>();
		<NonSerialized>
		Protected Friend helperWorkspaces As IDictionary(Of String, Pointer) = New Dictionary(Of String, Pointer)()


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
		''' Next 2 workspaces: used for:
		''' (a) Inference: holds activations for one layer only
		''' (b) Backprop: holds activation gradients for one layer only
		''' In both cases, they are opened and closed on every second layer
		''' </summary>
		Protected Friend Const WS_LAYER_ACT_1 As String = "WS_LAYER_ACT_1"
		Protected Friend Const WS_LAYER_ACT_2 As String = "WS_LAYER_ACT_2"

		''' <summary>
		''' Workspace for output methods that use OutputAdapter
		''' </summary>
		Protected Friend Const WS_OUTPUT_MEM As String = "WS_OUTPUT_MEM"

		''' <summary>
		''' Workspace for working memory in RNNs - opened and closed once per RNN time step
		''' </summary>
		Protected Friend Const WS_RNN_LOOP_WORKING_MEM As String = "WS_RNN_LOOP_WORKING_MEM"


		Protected Friend WS_LAYER_WORKING_MEM_CONFIG As WorkspaceConfiguration

		Protected Friend Shared ReadOnly WS_ALL_LAYERS_ACT_CONFIG As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.05).policyLearning(LearningPolicy.FIRST_LOOP).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()

		Protected Friend WS_LAYER_ACT_X_CONFIG As WorkspaceConfiguration

		Protected Friend Shared ReadOnly WS_RNN_LOOP_WORKING_MEM_CONFIG As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.05).policyReset(ResetPolicy.BLOCK_LEFT).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).build()


		Public Sub New(ByVal conf As MultiLayerConfiguration)
			Me.layerWiseConfigurations_Conflict = conf
			Me.defaultConfiguration_Conflict = conf.getConf(0).clone()

			'Working memory: should learn over course of: (a) full forward pass, and (b) full backward pass
			'Working memory should be opened once per layer and once per preprocessor, for each of forward and backward passes
			Dim numWorkingMem As Integer = 2 * (layerWiseConfigurations_Conflict.getConfs().size() + layerWiseConfigurations_Conflict.getInputPreProcessors().size())
			WS_LAYER_WORKING_MEM_CONFIG = getLayerWorkingMemWSConfig(numWorkingMem)
			WS_LAYER_ACT_X_CONFIG = getLayerActivationWSConfig(layerWiseConfigurations_Conflict.getConfs().size())
		End Sub

		Protected Friend Shared Function getLayerWorkingMemWSConfig(ByVal numWorkingMemCycles As Integer) As WorkspaceConfiguration
			Return WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(numWorkingMemCycles).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()
		End Function

		Protected Friend Shared Function getLayerActivationWSConfig(ByVal numLayers As Integer) As WorkspaceConfiguration
			'Activations memory: opened once per layer - for every second layer (preprocessors are within the loop).
			'Technically we could set learning to numLayers / 2, but will set to numLayers for simplicity, and also to
			' account for a backward pass
			Return WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.02).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(numLayers).policyReset(ResetPolicy.BLOCK_LEFT).policySpill(SpillPolicy.REALLOCATE).policyAllocation(AllocationPolicy.OVERALLOCATE).build()
		End Function

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
		''' Set the last ETL time in milliseconds, for informational/reporting purposes. Generally used internally. </summary>
		''' <param name="time">    ETL time </param>
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
		''' Initialize the network based on the configuration (a MultiLayerConfiguration in JSON format) and parameters array
		''' </summary>
		''' <param name="conf">   the configuration json </param>
		''' <param name="params"> the parameters for the network </param>
		Public Sub New(ByVal conf As String, ByVal params As INDArray)
			Me.New(MultiLayerConfiguration.fromJson(conf))
			init()
			Parameters = params
		End Sub


		''' <summary>
		''' Initialize the network based on the configuration and parameters array
		''' </summary>
		''' <param name="conf">   the configuration </param>
		''' <param name="params"> the parameters </param>
		Public Sub New(ByVal conf As MultiLayerConfiguration, ByVal params As INDArray)
			Me.New(conf)
			init()
			Parameters = params
		End Sub


		Protected Friend Overridable Sub intializeConfigurations()
			If layerWiseConfigurations_Conflict Is Nothing Then
				layerWiseConfigurations_Conflict = (New MultiLayerConfiguration.Builder()).build()
			End If

			If layers_Conflict Is Nothing Then
				layers_Conflict = New Layer(getnLayers() - 1){}
			End If

			If defaultConfiguration_Conflict Is Nothing Then
				defaultConfiguration_Conflict = (New NeuralNetConfiguration.Builder()).build()
			End If
		End Sub


		''' <summary>
		''' Perform layerwise pretraining for one epoch - see <seealso cref="pretrain(DataSetIterator, Integer)"/>
		''' </summary>
		Public Overridable Sub pretrain(ByVal iter As DataSetIterator)
			pretrain(iter, 1)
		End Sub

		''' <summary>
		''' Perform layerwise unsupervised training on all pre-trainable layers in the network (VAEs, Autoencoders, etc), for the specified
		''' number of epochs each. For example, if numEpochs=3, then layer 0 will be fit for 3 epochs, followed by layer 1
		''' for 3 epochs, and so on.<br>
		''' Note that pretraining will be performed on one layer after the other. To perform unsupervised training on a single layer,
		''' use <seealso cref="pretrainLayer(Integer, DataSetIterator)"/>
		''' </summary>
		''' <param name="iter"> Training data </param>
		Public Overridable Sub pretrain(ByVal iter As DataSetIterator, ByVal numEpochs As Integer)
			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If

			Dim i As Integer = 0
			Do While i < getnLayers()
				pretrainLayer(i, iter, numEpochs)
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Fit for one epoch - see <seealso cref="pretrainLayer(Integer, DataSetIterator, Integer)"/>
		''' </summary>
		Public Overridable Sub pretrainLayer(ByVal layerIdx As Integer, ByVal iter As DataSetIterator)
			pretrainLayer(layerIdx, iter, 1)
		End Sub

		''' <summary>
		''' Perform layerwise unsupervised training on a single pre-trainable layer in the network (VAEs, Autoencoders, etc)
		''' for the specified number of epochs<br>
		''' If the specified layer index (0 to numLayers - 1) is not a pretrainable layer, this is a no-op.
		''' </summary>
		''' <param name="layerIdx">  Index of the layer to train (0 to numLayers-1) </param>
		''' <param name="iter">      Training data </param>
		''' <param name="numEpochs"> Number of epochs to fit the specified layer for </param>
		Public Overridable Sub pretrainLayer(ByVal layerIdx As Integer, ByVal iter As DataSetIterator, ByVal numEpochs As Integer)
			Preconditions.checkState(numEpochs > 0, "Number of epochs (%s) must be a positive number", numEpochs)

			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If
			If layerIdx >= layers_Conflict.Length Then
				Throw New System.ArgumentException("Cannot pretrain layer: layerIdx (" & layerIdx & ") >= numLayers (" & layers_Conflict.Length & ")")
			End If

			Dim layer As Layer = layers_Conflict(layerIdx)
			If Not layer.PretrainLayer Then
				Return
			End If

			If numEpochs > 1 AndAlso Not iter.resetSupported() Then
				Throw New System.InvalidOperationException("Cannot fit multiple epochs (" & numEpochs & ") on an iterator that doesn't support resetting")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iter.hasNext() AndAlso iter.resetSupported() Then
				iter.reset()
			End If

			log.info("Starting unsupervised training on layer " & layerIdx & " for " & numEpochs & " epochs")
			For i As Integer = 0 To numEpochs - 1
				If i > 0 Then
					iter.reset()
				End If

				Do While iter.MoveNext()
					Dim [next] As DataSet = iter.Current
					input_Conflict = [next].Features
					pretrainLayer(layerIdx, input_Conflict)
				Loop
			Next i

			Dim ec As Integer = getLayer(layerIdx).conf().getEpochCount() + 1
			getLayer(layerIdx).conf().setEpochCount(ec)
		End Sub

		''' <summary>
		''' Perform layerwise unsupervised training on a single pre-trainable layer in the network (VAEs, Autoencoders, etc)<br>
		''' If the specified layer index (0 to numLayers - 1) is not a pretrainable layer, this is a no-op.
		''' </summary>
		''' <param name="layerIdx"> Index of the layer to train (0 to numLayers-1) </param>
		''' <param name="features"> Training data array </param>
		Public Overridable Sub pretrainLayer(ByVal layerIdx As Integer, ByVal features As INDArray)
			Input = features
			setLayerMaskArrays(Nothing, Nothing)

			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If
			If layerIdx >= layers_Conflict.Length Then
				Throw New System.ArgumentException("Cannot pretrain layer: layerIdx (" & layerIdx & ") >= numLayers (" & layers_Conflict.Length & ")")
			End If

			Dim workspaceMgr As LayerWorkspaceMgr
			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				workspaceMgr = LayerWorkspaceMgr.builder().defaultWorkspace(WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
			End If
			workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)

			Dim layer As Layer = layers_Conflict(layerIdx)
			If Not layer.PretrainLayer Then
				Return
			End If

			'Do forward pass to the layer to be pretrained
			Dim outputOfPrevLayer As INDArray
			If layerIdx = 0 Then
				outputOfPrevLayer = input_Conflict
			Else
				'Yes, this part of training - but we'll do forward psas as inference mode when doing layerwise training
				' to effectively freeze earlier layers and not apply dropout etc
				outputOfPrevLayer = outputOfLayerDetached(False, FwdPassType.STANDARD, layerIndex-1, features, Nothing, Nothing, Nothing)
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
				If layerWiseConfigurations_Conflict.getInputPreProcess(layerIdx) IsNot Nothing Then

					If input_Conflict.size(0) > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					outputOfPrevLayer = layerWiseConfigurations_Conflict.getInputPreProcess(layerIdx).preProcess(outputOfPrevLayer, CInt(input_Conflict.size(0)), LayerWorkspaceMgr.noWorkspaces(helperWorkspaces))
				End If

				layer.fit(outputOfPrevLayer, workspaceMgr)
			End Using
		End Sub

		Public Overridable Function batchSize() As Integer Implements org.deeplearning4j.nn.api.Model.batchSize
			'In 99+% of cases, the input and labels dimension 0 size should be identical
			'The only real exceptions: space to batch, and batch to space layers
			'In those cases, we should base it on the labels size, as this impacts gradient calculation
			If input_Conflict.size(0) > Integer.MaxValue OrElse labels_Conflict.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Return If(labels_Conflict Is Nothing, CInt(input_Conflict.size(0)), CInt(labels_Conflict.size(0)))
		End Function

		Public Overridable Function conf() As NeuralNetConfiguration
			Return defaultConfiguration_Conflict
		End Function

		Public Overridable WriteOnly Property Conf As NeuralNetConfiguration
			Set(ByVal conf As NeuralNetConfiguration)
				Throw New System.NotSupportedException()
			End Set
		End Property

		Public Overridable Function input() As INDArray Implements org.deeplearning4j.nn.api.Model.input
			Return input_Conflict
		End Function

		Public Overridable ReadOnly Property Optimizer As ConvexOptimizer Implements org.deeplearning4j.nn.api.Model.getOptimizer, NeuralNetwork.getOptimizer
			Get
				Return solver.Optimizer
			End Get
		End Property

		''' <summary>
		''' Get one parameter array for the network.<br>
		''' In MultiLayerNetwork, parameters are keyed like "0_W" and "0_b" to mean "weights of layer index 0" and "biases
		''' of layer index 0" respectively. Numbers increment sequentially, and the suffixes ("W", "b" etc) depend on the
		''' layer type, and are defined in the relevant parameter initializers for each layer.<br>
		''' Note that the returned INDArrays are views of the underlying network parameters, so modifications of the returned
		''' arrays will impact the parameters of the network.
		''' </summary>
		''' <param name="param"> the key of the parameter </param>
		''' <returns> The specified parameter array for the network </returns>
		''' <seealso cref= #paramTable() paramTable() method, for a map of all parameters </seealso>
		Public Overridable Function getParam(ByVal param As String) As INDArray Implements org.deeplearning4j.nn.api.Model.getParam
			'Get params for MultiLayerNetwork sub layers.
			Dim idx As Integer = param.IndexOf("_"c)
			If idx = -1 Then
				Throw New System.InvalidOperationException("Invalid param key: does not have layer separator: """ & param & """")
			End If
			Dim layerIdx As Integer = Integer.Parse(param.Substring(0, idx))
			Dim newKey As String = param.Substring(idx + 1)

			Return layers_Conflict(layerIdx).getParam(newKey)
		End Function

		''' <summary>
		''' Return a map of all parameters in the network. Parameter names are as described in <seealso cref="getParam(String)"/>.
		''' As per <seealso cref="getParam(String)"/> the returned arrays are views - modifications to these will impact
		''' the underlying network parameters </summary>
		''' <returns> A map of all parameters in the network </returns>
		Public Overridable Function paramTable() As IDictionary(Of String, INDArray)
			Return paramTable(False)
		End Function

		''' <summary>
		''' Returns a map of all parameters in the network as per <seealso cref="paramTable()"/>.<br>
		''' Optionally (with backpropParamsOnly=true) only the 'backprop' parameters are returned - that is, any parameters
		''' involved only in unsupervised layerwise pretraining not standard inference/backprop are excluded from the returned list. </summary>
		''' <param name="backpropParamsOnly"> If true, return backprop params only. If false: return all params </param>
		''' <returns> Parameters for the network </returns>
		Public Overridable Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			'Get all parameters from all layers
			Dim allParams As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For i As Integer = 0 To layers_Conflict.Length - 1
				Dim paramMap As IDictionary(Of String, INDArray) = layers_Conflict(i).paramTable(backpropParamsOnly)
				For Each entry As KeyValuePair(Of String, INDArray) In paramMap.SetOfKeyValuePairs()
					Dim newKey As String = i & "_" & entry.Key
					allParams(newKey) = entry.Value
				Next entry
			Next i
			Return allParams
		End Function

		''' <summary>
		''' Intended for internal use
		''' </summary>
		Public Overridable Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean Implements org.deeplearning4j.nn.api.Trainable.updaterDivideByMinibatch
			Dim idx As Integer = paramName.IndexOf("_"c)
			Dim layerIdx As Integer = Integer.Parse(paramName.Substring(0, idx))
			Dim subName As String = paramName.Substring(idx+1)
			Return getLayer(layerIdx).updaterDivideByMinibatch(subName)
		End Function

		''' <summary>
		''' Set the parameters of the netowrk. Note that the parameter keys must match the format as described in <seealso cref="getParam(String)"/>
		''' and <seealso cref="paramTable()"/>. Note that the values of the parameters used as an argument to this method are copied -
		''' i.e., it is safe to later modify/reuse the values in the provided paramTable without this impacting the network.
		''' </summary>
		''' <param name="paramTable">    Parameters to set </param>
		Public Overridable WriteOnly Property ParamTable As IDictionary(Of String, INDArray)
			Set(ByVal paramTable As IDictionary(Of String, INDArray))
				Dim currParamTable As IDictionary(Of String, INDArray) = Me.paramTable()
				If Not currParamTable.Keys.Equals(paramTable.Keys) Then
					Throw New System.ArgumentException("Cannot set param table: parameter keys do not match." & vbLf & "Current: " & currParamTable.Keys & vbLf & "To set: " & paramTable.Keys)
				End If
    
				For Each s As String In paramTable.Keys
					Dim curr As INDArray = currParamTable(s)
					Dim toSet As INDArray = Me.paramTable(s)
					If Not curr.shape().SequenceEqual(toSet.shape()) Then
						Throw New System.ArgumentException("Cannot set parameter table: parameter """ & s & """ shapes " & "do not match. Current = " & java.util.Arrays.toString(curr.shape()) & ", to set = " & java.util.Arrays.toString(toSet.shape()))
					End If
				Next s
    
				'Now that we've checked ALL params (to avoid leaving net in half-modified state)
				For Each s As String In paramTable.Keys
					Dim curr As INDArray = currParamTable(s)
					Dim toSet As INDArray = Me.paramTable(s)
					curr.assign(toSet)
				Next s
			End Set
		End Property

		''' <summary>
		''' Set the values of a single parameter. See <seealso cref="setParamTable(Map)"/> and <seealso cref="getParam(String)"/> for more
		''' details. </summary>
		''' <param name="key"> the key of the parameter to set </param>
		''' <param name="val"> the new values for the parameter </param>
		Public Overridable Sub setParam(ByVal key As String, ByVal val As INDArray) Implements org.deeplearning4j.nn.api.Model.setParam
			'Set params for MultiLayerNetwork sub layers.
			Dim idx As Integer = key.IndexOf("_"c)
			If idx = -1 Then
				Throw New System.InvalidOperationException("Invalid param key: not have layer separator: """ & key & """")
			End If
			Dim layerIdx As Integer = Integer.Parse(key.Substring(0, idx))
			Dim newKey As String = key.Substring(idx + 1)

			layers_Conflict(layerIdx).setParam(newKey, val)
		End Sub

		''' <summary>
		''' Get the configuration for the network </summary>
		''' <returns> Network configuration </returns>
		Public Overridable Property LayerWiseConfigurations As MultiLayerConfiguration
			Get
				Return layerWiseConfigurations_Conflict
			End Get
			Set(ByVal layerWiseConfigurations As MultiLayerConfiguration)
				Me.layerWiseConfigurations_Conflict = layerWiseConfigurations
			End Set
		End Property


		''' <summary>
		''' Initialize the MultiLayerNetwork. This should be called once before the network is used.
		''' This is functionally equivalent to calling {@code init(null, false)}. </summary>
		''' <seealso cref= MultiLayerNetwork#init(INDArray, boolean) </seealso>
		Public Overridable Sub init() Implements org.deeplearning4j.nn.api.Model.init, NeuralNetwork.init
			init(Nothing, False)
		End Sub

		''' <summary>
		''' Initialize the MultiLayerNetwork, optionally with an existing parameters array.
		''' If an existing parameters array is specified, it will be used (and the values will not be modified) in the network;
		''' if no parameters array is specified, parameters will be initialized randomly according to the network configuration.
		''' </summary>
		''' <param name="parameters">              Network parameter. May be null. If null: randomly initialize. </param>
		''' <param name="cloneParametersArray">    Whether the parameter array (if any) should be cloned, or used directly </param>
		Public Overridable Sub init(ByVal parameters As INDArray, ByVal cloneParametersArray As Boolean)
			If layerWiseConfigurations_Conflict Is Nothing OrElse layers_Conflict Is Nothing Then
				intializeConfigurations()
			End If
			If initCalled_Conflict Then
				Return
			End If

			Dim netDtype As DataType = LayerWiseConfigurations.getDataType()
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


			If layerMap Is Nothing Then
				layerMap = New LinkedHashMap(Of String, Layer)()
			End If

			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() Is Nothing Then
				layerWiseConfigurations_Conflict.setTrainingWorkspaceMode(WorkspaceMode.NONE)
			End If

			If layerWiseConfigurations_Conflict.getInferenceWorkspaceMode() Is Nothing Then
				layerWiseConfigurations_Conflict.setInferenceWorkspaceMode(WorkspaceMode.NONE)
			End If

			If layerWiseConfigurations_Conflict.getCacheMode() Is Nothing Then
				layerWiseConfigurations_Conflict.setCacheMode(CacheMode.NONE)
			End If

			OneTimeLogger.info(log, "Starting MultiLayerNetwork with WorkspaceModes set to [training: {}; inference: {}], cacheMode set to [{}]", layerWiseConfigurations_Conflict.getTrainingWorkspaceMode(), layerWiseConfigurations_Conflict.getInferenceWorkspaceMode(), layerWiseConfigurations_Conflict.getCacheMode())

			Dim nLayers As Integer = getnLayers()

			If nLayers < 1 Then
				Throw New System.InvalidOperationException("Unable to create network: number of layers is less than 1")
			End If

			If Me.layers_Conflict Is Nothing OrElse Me.layers_Conflict(0) Is Nothing Then
				If Me.layers_Conflict Is Nothing Then
					Me.layers_Conflict = New Layer(nLayers - 1){}
				End If

				'First: Work out total length of params
				Dim paramLength As Long = 0
				Dim nParamsPerLayer As val = New Long(nLayers - 1){}
				For i As Integer = 0 To nLayers - 1
					Dim conf As NeuralNetConfiguration = layerWiseConfigurations_Conflict.getConf(i)
					conf.getLayer().setDataType(netDtype)
					nParamsPerLayer(i) = conf.getLayer().initializer().numParams(conf)
					paramLength += nParamsPerLayer(i)
				Next i

				'Create parameters array, if required
				Dim initializeParams As Boolean
				If parameters IsNot Nothing Then
					If Not parameters.RowVectorOrScalar Then
						Throw New System.ArgumentException("Invalid parameters: should be a row vector")
					End If
					If parameters.length() <> paramLength Then
						Throw New System.ArgumentException("Invalid parameters: expected length " & paramLength & ", got length " & parameters.length())
					End If

					If cloneParametersArray Then
						flattenedParams = parameters.dup()
					Else
						flattenedParams = parameters
					End If

					initializeParams = False
				ElseIf paramLength > 0 Then
					flattenedParams = Nd4j.create(netDtype, 1, paramLength)
					initializeParams = True
				Else
					'Edge case: 0 params in network
					flattenedParams = Nothing
					initializeParams = False
				End If

				'Set RNG seed, for repeatability between initializations when set
				If initializeParams Then
					Nd4j.Random.setSeed(DefaultConfiguration.getSeed())
				End If

				' construct multi-layer
				Dim paramCountSoFar As Long = 0
				For i As Integer = 0 To nLayers - 1
					Dim paramsView As INDArray
					If nParamsPerLayer(i) > 0 Then
						paramsView = flattenedParams.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramCountSoFar, paramCountSoFar + nParamsPerLayer(i)))
					Else
						paramsView = Nothing
					End If
					paramCountSoFar += nParamsPerLayer(i)

					Dim conf As NeuralNetConfiguration = layerWiseConfigurations_Conflict.getConf(i)
					layers_Conflict(i) = conf.getLayer().instantiate(conf, trainingListeners, i, paramsView, initializeParams, netDtype)
					layerMap.put(conf.getLayer().getLayerName(), layers_Conflict(i))
				Next i
				initCalled_Conflict = True
			End If

			'Set parameters in MultiLayerNetwork.defaultConfiguration for later use in BaseOptimizer.setupSearchState() etc
			defaultConfiguration_Conflict.clearVariables()
			Dim variables As IList(Of String) = defaultConfiguration_Conflict.variables(False)
			For i As Integer = 0 To layers_Conflict.Length - 1
				If layers_Conflict(i) Is Nothing Then
					Throw New System.InvalidOperationException("Encountered null layer during initialization for layer " & i & ": " & layerWiseConfigurations_Conflict.getConf(i).getLayer().GetType().Name & " initialization " & "returned null layer?")
				End If

				For Each s As String In layers_Conflict(i).conf().variables()
					variables.Add(i & "_" & s)
				Next s
			Next i

			' now we init solver & optimizer
			If solver Is Nothing Then
				Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
					solver.initOptimizer()
				End Using
			End If

			'Mark that input modification is allowed.
			'TODO When is it safe to NOT skip the very first layer? It's not always safe...
			' For example dropout + iterating over List<DataSet> that is used for multiple epochs...
			For i As Integer = 1 To layers_Conflict.Length - 1
				layers_Conflict(i).allowInputModification(True)
			Next i

			synchronizeIterEpochCounts()
		End Sub

		''' <summary>
		''' This method allows you to specificy GradientsAccumulator instance to be used with this model<br>
		''' <br>
		''' PLEASE NOTE: Do not use this method unless you understand how to use GradientsAccumulator & updates sharing.<br>
		''' PLEASE NOTE: Do not use this method on standalone model
		''' </summary>
		''' <param name="accumulator">    Gradient accumulator to use for the network </param>
		Public Overridable WriteOnly Property GradientsAccumulator As GradientsAccumulator
			Set(ByVal accumulator As GradientsAccumulator)
				If Not InitCalled Then
					init()
				End If
    
				If solver Is Nothing Then
					Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
					End Using
				End If
    
				solver.Optimizer.GradientsAccumulator = accumulator
			End Set
		End Property

		Public Overridable ReadOnly Property InitCalled As Boolean
			Get
				Return initCalled_Conflict
			End Get
		End Property

		''' <summary>
		''' This method: initializes the flattened gradients array (used in backprop) and sets the appropriate subset in all layers.
		''' As a general rule, this shouldn't ever need to be called manually when doing training via fit(DataSet) or fit(DataSetIterator)
		''' </summary>
		Public Overridable Sub initGradientsView()
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				If layers_Conflict Is Nothing Then
					init()
				End If

				Dim nLayers As Integer = layers_Conflict.Length

				'First: Work out total length of params
				Dim paramLength As Long = 0
				Dim nParamsPerLayer As val = New Long(nLayers - 1){}
				For i As Integer = 0 To nLayers - 1
					Dim conf As NeuralNetConfiguration = layerWiseConfigurations_Conflict.getConf(i)
					nParamsPerLayer(i) = conf.getLayer().initializer().numParams(conf)
					paramLength += nParamsPerLayer(i)
				Next i

				If paramLength > 0 Then
					flattenedGradients = Nd4j.create(flattenedParams.dataType(), New Long(){1, paramLength}, "f"c) 'No need to initialize, as each layer will do it each iteration anyway
				End If

				Dim paramsSoFar As Long = 0
				For i As Integer = 0 To layers_Conflict.Length - 1
					If nParamsPerLayer(i) = 0 Then
						Continue For 'This layer doesn't have any parameters...
					End If
					Dim thisLayerGradView As INDArray = flattenedGradients.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramsSoFar, paramsSoFar + nParamsPerLayer(i)))
					layers_Conflict(i).BackpropGradientsViewArray = thisLayerGradView
					paramsSoFar += nParamsPerLayer(i)
				Next i
			End Using
		End Sub

		Protected Friend Overridable Function activationFromPrevLayer(ByVal curr As Integer, ByVal input As INDArray, ByVal training As Boolean, ByVal mgr As LayerWorkspaceMgr) As INDArray
			If LayerWiseConfigurations.getInputPreProcess(curr) IsNot Nothing Then
				input = LayerWiseConfigurations.getInputPreProcess(curr).preProcess(input, InputMiniBatchSize, mgr)
			End If

			Dim ret As INDArray = layers_Conflict(curr).activate(input, training, mgr)
			Return ret
		End Function

		''' <summary>
		''' Calculate activation for few layers at once. Suitable for autoencoder partial activation.
		''' 
		''' In example: in 10-layer deep autoencoder, layers 0 - 4 inclusive are used for encoding part, and layers 5-9 inclusive are used for decoding part.
		''' </summary>
		''' <param name="from"> first layer to be activated, inclusive </param>
		''' <param name="to"> last layer to be activated, inclusive </param>
		''' <returns> the activation from the last layer </returns>
		Public Overridable Function activateSelectedLayers(ByVal from As Integer, ByVal [to] As Integer, ByVal input As INDArray) As INDArray
			If input Is Nothing Then
				Throw New System.InvalidOperationException("Unable to perform activation; no input found")
			End If
			If from < 0 OrElse from >= layers_Conflict.Length OrElse from >= [to] Then
				Throw New System.InvalidOperationException("Unable to perform activation; FROM is out of layer space")
			End If
			If [to] < 1 OrElse [to] >= layers_Conflict.Length Then
				Throw New System.InvalidOperationException("Unable to perform activation; TO is out of layer space")
			End If

			Try
				Dim mgr As LayerWorkspaceMgr = LayerWorkspaceMgr.noWorkspaces(helperWorkspaces) 'TODO

				Dim res As INDArray = input
				For l As Integer = from To [to]
					res = Me.activationFromPrevLayer(l, res, False, mgr)
				Next l
				Return res
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Compute all layer activations, from input to output of the output layer.
		''' Note that the input is included in the list: thus feedForward(in,train).get(0) is the inputs,
		''' .get(1) is the activations of layer 0, and so on.
		''' </summary>
		''' <param name="train"> Training: if true, perform forward pass/inference at training time. Usually, inference is performed
		'''              with train = false. This impacts whether dropout etc is applied or not. </param>
		''' <returns> The list of activations for each layer, including the input </returns>
		Public Overridable Function feedForward(ByVal input As INDArray, ByVal train As Boolean) As IList(Of INDArray)
			Me.Input = input
			Return feedForward(train)
		End Function

		''' <summary>
		''' Compute activations from input to output of the output layer.
		''' As per <seealso cref="feedForward(INDArray, Boolean)"/> but using the inputs that have previously been set using <seealso cref="setInput(INDArray)"/>
		''' </summary>
		''' <returns> the list of activations for each layer </returns>
		Public Overridable Function feedForward(ByVal train As Boolean) As IList(Of INDArray)
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, layers_Conflict.Length-1, input_Conflict, mask_Conflict, Nothing, True)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Perform feed-forward, optionally (not) clearing the layer input arrays.<br>
		''' Note: when using clearInputs=false, there can be some performance and memory overhead: this is because the arrays are
		''' defined outside of workspaces (which are enabled by default) - otherwise, old/invalidated arrays could still be
		''' accessed after calling this method. Consequently: Don't use clearInputs=false unless you have a use case that
		''' requires them to remain after feed-forward has been completed
		''' </summary>
		''' <param name="train">       training mode (true) or test mode (false) </param>
		''' <param name="clearInputs"> If false: don't clear the layer inputs </param>
		''' <returns> Activations from feed-forward </returns>
		Public Overridable Function feedForward(ByVal train As Boolean, ByVal clearInputs As Boolean) As IList(Of INDArray)
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, layers_Conflict.Length-1, input_Conflict, mask_Conflict, Nothing, clearInputs)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Compute the activations from the input to the specified layer.<br>
		''' To compute activations for all layers, use feedForward(...) methods<br>
		''' Note: output list includes the original input. So list.get(0) is always the original input, and
		''' list.get(i+1) is the activations of the ith layer. </summary>
		''' <param name="layerNum"> Index of the last layer to calculate activations for. Layers are zero-indexed.
		'''                 feedForwardToLayer(i,input) will return the activations for layers 0..i (inclusive) </param>
		''' <param name="input"> Input to the network </param>
		''' <returns> list of activations. </returns>
		Public Overridable Function feedForwardToLayer(ByVal layerNum As Integer, ByVal input As INDArray) As IList(Of INDArray)
			Try
				Return ffToLayerActivationsDetached(False, FwdPassType.STANDARD, False, layerNum, input, mask_Conflict, Nothing, True)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Compute the activations from the input to the specified layer.<br>
		''' To compute activations for all layers, use feedForward(...) methods<br>
		''' Note: output list includes the original input. So list.get(0) is always the original input, and
		''' list.get(i+1) is the activations of the ith layer. </summary>
		''' <param name="layerNum"> Index of the last layer to calculate activations for. Layers are zero-indexed.
		'''                 feedForwardToLayer(i,input) will return the activations for layers 0..i (inclusive) </param>
		''' <param name="input"> Input to the network </param>
		''' <param name="train"> true for training, false for test (i.e., false if using network after training) </param>
		''' <returns> list of activations. </returns>
		Public Overridable Function feedForwardToLayer(ByVal layerNum As Integer, ByVal input As INDArray, ByVal train As Boolean) As IList(Of INDArray)
			Try
				Dim layerVertexIdx As Integer = layers_Conflict(layerNum).Index
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, layerVertexIdx, input, mask_Conflict, Nothing, True)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		''' Compute the activations from the input to the specified layer, using the currently set input for the network.<br>
		''' To compute activations for all layers, use feedForward(...) methods<br>
		''' Note: output list includes the original input. So list.get(0) is always the original input, and
		''' list.get(i+1) is the activations of the ith layer. </summary>
		''' <param name="layerNum"> Index of the last layer to calculate activations for. Layers are zero-indexed.
		'''                 feedForwardToLayer(i,input) will return the activations for layers 0..i (inclusive) </param>
		''' <param name="train"> true for training, false for test (i.e., false if using network after training) </param>
		''' <returns> list of activations. </returns>
		Public Overridable Function feedForwardToLayer(ByVal layerNum As Integer, ByVal train As Boolean) As IList(Of INDArray)
			Try
				Return ffToLayerActivationsDetached(train, FwdPassType.STANDARD, False, layerNum, input_Conflict, mask_Conflict, Nothing, True)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function


		Protected Friend Overridable Sub validateArrayWorkspaces(ByVal mgr As LayerWorkspaceMgr, ByVal array As INDArray, ByVal arrayType As ArrayType, ByVal layerIdx As Integer, ByVal isPreprocessor As Boolean, ByVal op As String)
			Try
				'if the layer is a pre processor be a bit more flexible with migration, for strict layers
				'throw exception (mainly for performance reasons)
				mgr.validateArrayLocation(arrayType, array, isPreprocessor, layerIdx > 0)
			Catch e As ND4JWorkspaceException
				Dim layerName As String = layers_Conflict(layerIdx).conf().getLayer().getLayerName()
				Dim clazz As String
				If isPreprocessor Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					clazz = layerWiseConfigurations_Conflict.getInputPreProcess(layerIdx).GetType().FullName
				Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					clazz = layers_Conflict(layerIdx).GetType().FullName
				End If
				Throw New System.InvalidOperationException(op & ": array (" & arrayType & ") workspace validation failed (" & (If(isPreprocessor, "preprocessor", "layer ")) + layerIdx + (If(layerName IsNot Nothing, " - layer name """ & layerName & """", "")) & " - class: " & clazz & ") - array is defined in incorrect workspace", e)
			End Try
		End Sub

		''' <summary>
		''' Feed-forward through the network - returning all array activations in a list, detached from any workspace.
		''' Note that no workspace should be active externally when calling this method (an exception will be thrown
		''' if a workspace is open externally)
		''' </summary>
		''' <param name="train">             Training mode (true) or test/inference mode (false) </param>
		''' <param name="fwdPassType">       Type of forward pass to perform (STANDARD or RNN_ACTIVATE_WITH_STORED_STATE only) </param>
		''' <param name="storeLastForTBPTT"> ONLY used if fwdPassType == FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE </param>
		''' <param name="layerIndex">        Index (inclusive) to stop forward pass at. For all layers, use numLayers-1 </param>
		''' <param name="input">             Input to the network </param>
		''' <param name="fMask">             Feature mask array. May be null. </param>
		''' <param name="lMask">             Label mask array. May be null. </param>
		''' <param name="clearInputs">       Whether the layer inputs should be cleared </param>
		''' <returns> List of activations (including the input), detached from any workspace </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected synchronized List<org.nd4j.linalg.api.ndarray.INDArray> ffToLayerActivationsDetached(boolean train, @NonNull FwdPassType fwdPassType, boolean storeLastForTBPTT, int layerIndex, @NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray fMask, org.nd4j.linalg.api.ndarray.INDArray lMask, boolean clearInputs)
		Protected Friend Overridable Function ffToLayerActivationsDetached(ByVal train As Boolean, ByVal fwdPassType As FwdPassType, ByVal storeLastForTBPTT As Boolean, ByVal layerIndex As Integer, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray, ByVal clearInputs As Boolean) As IList(Of INDArray)
			SyncLock Me
				Me.Input = input
				setLayerMaskArrays(fMask, lMask)
        
				'Verify that no workspace is open externally
				WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active in ffToLayerActivationsDetached")
        
				Dim workspaceMgr As LayerWorkspaceMgr
				Dim wsm As WorkspaceMode = (If(train, layerWiseConfigurations_Conflict.getTrainingWorkspaceMode(), layerWiseConfigurations_Conflict.getInferenceWorkspaceMode()))
				If wsm = WorkspaceMode.NONE Then
					workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
				Else
					workspaceMgr = LayerWorkspaceMgr.builder().noWorkspaceFor(ArrayType.ACTIVATIONS).with(ArrayType.INPUT, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
        
					If input.isAttached() Then
						'Don't leverage out of async DataSetIterator workspaces
						workspaceMgr.NoLeverageOverride = input.data().getParentWorkspace().getId()
					End If
        
					If Not clearInputs Then
						workspaceMgr.ScopedOutFor = ArrayType.INPUT
					End If
				End If
				workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)
        
				Dim [out] As IList(Of INDArray) = New List(Of INDArray)()
				[out].Add(workspaceMgr.leverageTo(ArrayType.INPUT, input)) 'Should  be unnecessary (and no op), if layer is implemented correctly
        
				For i As Integer = 0 To layerIndex
					Using wsFFWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						If LayerWiseConfigurations.getInputPreProcess(i) IsNot Nothing Then
							input = LayerWiseConfigurations.getInputPreProcess(i).preProcess(input, InputMiniBatchSize, workspaceMgr)
							'Validation: Exception if invalid (bad preprocessor implementation)
							validateArrayWorkspaces(workspaceMgr, input, ArrayType.ACTIVATIONS, i, True, "Feed forward to layer (inference)")
						End If
        
						If fwdPassType = FwdPassType.STANDARD Then
							input = layers_Conflict(i).activate(input, train, workspaceMgr)
						ElseIf fwdPassType = FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE Then
							If TypeOf layers_Conflict(i) Is RecurrentLayer Then
								input = DirectCast(layers_Conflict(i), RecurrentLayer).rnnActivateUsingStoredState(input, train, storeLastForTBPTT, workspaceMgr)
							ElseIf TypeOf layers_Conflict(i) Is BaseWrapperLayer AndAlso TypeOf (DirectCast(layers_Conflict(i), BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
								Dim rl As RecurrentLayer = DirectCast((DirectCast(layers_Conflict(i), BaseWrapperLayer)).getUnderlying(), RecurrentLayer)
								input = rl.rnnActivateUsingStoredState(input, train,storeLastForTBPTT, workspaceMgr)
							ElseIf TypeOf layers_Conflict(i) Is MultiLayerNetwork Then
								Dim temp As IList(Of INDArray) = DirectCast(layers_Conflict(i), MultiLayerNetwork).rnnActivateUsingStoredState(input, train, storeLastForTBPTT)
								input = temp(temp.Count - 1)
							Else
								input = layers_Conflict(i).activate(input, train, workspaceMgr)
							End If
						Else
							Throw New System.InvalidOperationException("Forward pass type not supported for this method: " & fwdPassType)
						End If
        
						'Validation: Exception if invalid (bad layer implementation)
						validateArrayWorkspaces(workspaceMgr, input, ArrayType.ACTIVATIONS, i, False, "Feed forward to layer (inference)")
        
						[out].Add(input)
					End Using
					If clearInputs Then
						layers_Conflict(i).clear()
					End If
				Next i
        
				Return [out]
			End SyncLock
		End Function

		''' <summary>
		''' Feed-forward through the network at training time - returning a list of all activations in a workspace (WS_ALL_LAYERS_ACT)
		''' if workspaces are enabled for training; or detached if no workspaces are used.<br>
		''' Note: if using workspaces for training, this method requires that WS_ALL_LAYERS_ACT is open externally.<br>
		''' If using NO workspaces, requires that no external workspace is open<br>
		''' Note that this method does NOT clear the inputs to each layer - instead, they are in the WS_ALL_LAYERS_ACT workspace
		''' for use in later backprop.
		''' </summary>
		''' <param name="layerIndex">        Index (inclusive) to stop forward pass at. For all layers, use numLayers-1 </param>
		''' <param name="fwdPassType">       Type of forward pass to perform (STANDARD or RNN_ACTIVATE_WITH_STORED_STATE only) </param>
		''' <param name="storeLastForTBPTT"> ONLY used if fwdPassType == FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE </param>
		''' <param name="input">             Input to network </param>
		''' <param name="fMask">             Feature mask array. May be null </param>
		''' <param name="lMask">             Label mask aray. May be null.
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected synchronized List<org.nd4j.linalg.api.ndarray.INDArray> ffToLayerActivationsInWs(int layerIndex, @NonNull FwdPassType fwdPassType, boolean storeLastForTBPTT, @NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray fMask, org.nd4j.linalg.api.ndarray.INDArray lMask)
		Protected Friend Overridable Function ffToLayerActivationsInWs(ByVal layerIndex As Integer, ByVal fwdPassType As FwdPassType, ByVal storeLastForTBPTT As Boolean, ByVal input As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As IList(Of INDArray)
			SyncLock Me
				Me.Input = input
				setLayerMaskArrays(fMask, lMask)
        
				Dim workspaceMgr As LayerWorkspaceMgr
				If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
					WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active in ffToLayerActivationsInWs when training workspace is set to NONE")
					workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
				Else
					workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
        
					If input.isAttached() Then
						'Don't leverage out of async DataSetIterator workspaces
						workspaceMgr.NoLeverageOverride = input.data().getParentWorkspace().getId()
					End If
        
					If layerWiseConfigurations_Conflict.getCacheMode() <> CacheMode.NONE Then
						'For now: store cache mode activations in activations workspace
						workspaceMgr.setWorkspace(ArrayType.FF_CACHE, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG)
						workspaceMgr.setWorkspace(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG)
					End If
        
					WorkspaceUtils.assertOpenAndActive(WS_ALL_LAYERS_ACT, "ffToLayerActivationsInWs method requires workspace WS_ALL_LAYERS_ACT to be open")
				End If
				workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)
        
				Dim [out] As IList(Of INDArray) = New List(Of INDArray)()
				[out].Add(workspaceMgr.leverageTo(ArrayType.INPUT, input)) 'Probably unnecessary usually
        
				Dim traceLog As Boolean = log.isTraceEnabled()
        
				For i As Integer = 0 To layerIndex
					Using wsFFWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
						If LayerWiseConfigurations.getInputPreProcess(i) IsNot Nothing Then
							input = LayerWiseConfigurations.getInputPreProcess(i).preProcess(input, InputMiniBatchSize, workspaceMgr)
							'Validation: Exception if invalid (bad preprocessor implementation)
							validateArrayWorkspaces(workspaceMgr, input, ArrayType.ACTIVATIONS, i, True, "Feed forward to layer (training)")
						End If
        
						If traceLog Then
							log.trace("About to forward pass: {} - {}", i, layers_Conflict(i).GetType().Name)
						End If
        
						If fwdPassType = FwdPassType.STANDARD Then
							input = layers_Conflict(i).activate(input, True, workspaceMgr)
						ElseIf fwdPassType = FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE Then
							If TypeOf layers_Conflict(i) Is RecurrentLayer Then
								input = DirectCast(layers_Conflict(i), RecurrentLayer).rnnActivateUsingStoredState(input, True, storeLastForTBPTT, workspaceMgr)
							ElseIf TypeOf layers_Conflict(i) Is BaseWrapperLayer AndAlso TypeOf (DirectCast(layers_Conflict(i), BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
								Dim rl As RecurrentLayer = DirectCast((DirectCast(layers_Conflict(i), BaseWrapperLayer)).getUnderlying(), RecurrentLayer)
								input = rl.rnnActivateUsingStoredState(input, True, storeLastForTBPTT, workspaceMgr)
							ElseIf TypeOf layers_Conflict(i) Is MultiLayerNetwork Then
								Dim temp As IList(Of INDArray) = DirectCast(layers_Conflict(i), MultiLayerNetwork).rnnActivateUsingStoredState(input, True, storeLastForTBPTT)
								input = temp(temp.Count - 1)
							Else
								input = layers_Conflict(i).activate(input, True, workspaceMgr)
							End If
						Else
							Throw New System.InvalidOperationException("FwdPassType not supported for this method: " & fwdPassType)
						End If
        
						If input Is Nothing Then
							Throw New System.InvalidOperationException("Layer " & i & " returned null activations")
						End If
        
						'Validation: Exception if invalid (bad layer implementation)
						validateArrayWorkspaces(workspaceMgr, input, ArrayType.ACTIVATIONS, i, False, "Feed forward to layer (training)")
						validateArrayWorkspaces(workspaceMgr, layers_Conflict(i).input(), ArrayType.INPUT, i, False, "Feed forward to layer (training)")
        
						[out].Add(input)
        
						If traceLog Then
							log.trace("Completed forward pass: {} - {}", i, layers_Conflict(i).GetType().Name)
						End If
					End Using
				Next i
        
				Return [out]
			End SyncLock
		End Function

		''' <summary>
		''' Provide the output of the specified layer, detached from any workspace. This is most commonly used at inference/test
		''' time, and is more memory efficient than <seealso cref="ffToLayerActivationsDetached(Boolean, FwdPassType, Boolean, Integer, INDArray, INDArray, INDArray, Boolean)"/>
		''' and <seealso cref="ffToLayerActivationsInWs(Integer, FwdPassType, Boolean, INDArray, INDArray, INDArray)"/>.<br>
		''' This method clears all layer inputs.
		''' 
		''' NOTE: in general, no workspaces should be activated externally for this method!
		''' This method handles the workspace activation as required
		''' </summary>
		''' <param name="train">             Training mode (true) or test/inference mode (false) </param>
		''' <param name="fwdPassType">       Type of forward pass to perform (STANDARD, RNN_TIMESTEP or RNN_ACTIVATE_WITH_STORED_STATE) </param>
		''' <param name="layerIndex">        Index (inclusive) to stop forward pass at. For all layers, use numLayers-1 </param>
		''' <param name="input">             Input to the network </param>
		''' <param name="featureMask">       Input/feature mask array. May be null. </param>
		''' <param name="labelsMask">        Labels mask array. May be null </param>
		''' <param name="outputWorkspace">   Optional - if provided, outputs should be placed in this workspace. NOTE: this workspace
		'''                          must be open </param>
		''' <returns>                  Output of the specified layer, detached from any workspace </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected org.nd4j.linalg.api.ndarray.INDArray outputOfLayerDetached(boolean train, @NonNull FwdPassType fwdPassType, int layerIndex, @NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray featureMask, org.nd4j.linalg.api.ndarray.INDArray labelsMask, org.nd4j.linalg.api.memory.MemoryWorkspace outputWorkspace)
		Protected Friend Overridable Function outputOfLayerDetached(ByVal train As Boolean, ByVal fwdPassType As FwdPassType, ByVal layerIndex As Integer, ByVal input As INDArray, ByVal featureMask As INDArray, ByVal labelsMask As INDArray, ByVal outputWorkspace As MemoryWorkspace) As INDArray
			Me.Input = input
			setLayerMaskArrays(featureMask, labelsMask)

	'        
	'        Idea here: we want to minimize memory, and return only the final array
	'        Approach to do this: keep activations in memory only as long as we need them.
	'        In MultiLayerNetwork, the output activations of layer X are used as input to layer X+1
	'        Which means: the workspace for layer X has to be open for both layers X and X+1 forward pass.
	'
	'        Here, we'll use two workspaces for activations:
	'        1. For even index layers, activations WS that opens on start of even layer fwd pass, closes at end of odd layer fwd pass
	'        2. For odd index layers, activations WS that opens on start of odd layer fwd pass, closes at end of even layer fwd pass
	'
	'        Additionally, we'll reconfigure the workspace manager for the *final* layer, so that we don't have to detach
	'         
			If outputWorkspace Is Nothing OrElse TypeOf outputWorkspace Is DummyWorkspace Then
				WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active in outputOfLayerDetached", True)
			Else
				Preconditions.checkState(outputWorkspace.ScopeActive, "Workspace """ & outputWorkspace.Id & """ was provided for the network/layer outputs. When provided, this workspace must be opened before " & "calling the output method; furthermore, closing the workspace is the responsibility of the user")
			End If

			Dim mgrEven As LayerWorkspaceMgr
			Dim mgrOdd As LayerWorkspaceMgr

			Dim wsm As WorkspaceMode = If(train, layerWiseConfigurations_Conflict.getTrainingWorkspaceMode(), layerWiseConfigurations_Conflict.getInferenceWorkspaceMode())
			If wsm = WorkspaceMode.NONE Then
				mgrEven = LayerWorkspaceMgr.noWorkspaces()
				mgrOdd = mgrEven

				'Check for external workspace - doesn't make sense to have one with workspace mode NONE
				If outputWorkspace IsNot Nothing AndAlso Not (TypeOf outputWorkspace Is DummyWorkspace) Then
					Throw New System.InvalidOperationException("Workspace """ & outputWorkspace.Id & """ was provided for the network/layer outputs, however " & (If(train, "training", "inference")) & " workspace mode is set to NONE. Cannot put output activations into the specified workspace if" & "workspaces are disabled for the network. use getConfiguration().setTraining/InferenceWorkspaceMode(WorkspaceMode.ENABLED)")
				End If
			Else
				mgrEven = LayerWorkspaceMgr.builder().with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.ACTIVATIONS, WS_LAYER_ACT_1, WS_LAYER_ACT_X_CONFIG).with(ArrayType.INPUT, WS_LAYER_ACT_2, WS_LAYER_ACT_X_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

				mgrOdd = LayerWorkspaceMgr.builder().with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.ACTIVATIONS, WS_LAYER_ACT_2, WS_LAYER_ACT_X_CONFIG).with(ArrayType.INPUT, WS_LAYER_ACT_1, WS_LAYER_ACT_X_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()
			End If
			mgrEven.setHelperWorkspacePointers(helperWorkspaces)
			mgrOdd.setHelperWorkspacePointers(helperWorkspaces)

			Dim wsActCloseNext As MemoryWorkspace = Nothing
			Dim temp As MemoryWorkspace = Nothing
			Dim initialWorkspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			Dim traceLog As Boolean = log.isTraceEnabled()

			Dim t As Exception = Nothing
			Try
				For i As Integer = 0 To layerIndex
					Dim mgr As LayerWorkspaceMgr = (If(i Mod 2 = 0, mgrEven, mgrOdd))

					If traceLog Then
						log.trace("About to forward pass: {} - {}", i, layers_Conflict(i).GetType().Name)
					End If

					'Edge case: for first layer with dropout, inputs can't be in previous workspace (as it hasn't been opened yet)
					'Hence: put inputs in working memory
					If i = 0 AndAlso wsm <> WorkspaceMode.NONE Then
						mgr.setWorkspace(ArrayType.INPUT, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG)
					End If

					Using wsFFWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM) 'Working memory: opened/closed once per layer
						'Activations workspaces: opened/closed every second layer.
						'So mgrEven (WS_LAYER_ACT_1) open at start of 0, 2, 4, 8; closed at end of 1, 3, 5, 7 etc
						'and mgrOdd (WS_LAYER_ACT_2) opened at start of 1, 3, 5, 7; closed at end of 2, 4, 6, 8 etc
						temp = mgr.notifyScopeEntered(ArrayType.ACTIVATIONS)

						'Note that because we're opening activation workspaces not in a simple nested order, we'll manually
						' override the previous workspace setting. Otherwise, when we close these workspaces, the "current"
						' workspace may be set to the incorrect one
						temp.PreviousWorkspace = initialWorkspace


						If i = 0 AndAlso input.isAttached() Then
							'Don't leverage out of async DataSetIterator workspaces
							mgr.NoLeverageOverride = input.data().getParentWorkspace().getId()
						End If

						If LayerWiseConfigurations.getInputPreProcess(i) IsNot Nothing Then
							input = LayerWiseConfigurations.getInputPreProcess(i).preProcess(input, InputMiniBatchSize, mgr)
							'Validation: Exception if invalid (bad preprocessor implementation)
							validateArrayWorkspaces(mgr, input, ArrayType.ACTIVATIONS, i, True, "Output of layer (inference)")
						End If

						If i = layerIndex Then
							If outputWorkspace IsNot Nothing AndAlso Not (TypeOf outputWorkspace Is DummyWorkspace) Then
								'Place activations in user-specified workspace
								mgr.setWorkspace(ArrayType.ACTIVATIONS, outputWorkspace.Id, outputWorkspace.WorkspaceConfiguration)
							Else
								'Final activations: should be detached
								mgr.ScopedOutFor = ArrayType.ACTIVATIONS
							End If
						End If

						If fwdPassType = FwdPassType.STANDARD Then
							'Standard feed-forward case
							If i > 0 AndAlso ConvolutionUtils.layerHasConvolutionLayout(layers_Conflict(i - 1).conf().getLayer()) AndAlso ConvolutionUtils.layerHasConvolutionLayout(layers_Conflict(i).conf().getLayer()) Then

								Dim preLayerFormat As CNN2DFormat = ConvolutionUtils.getFormatForLayer(layers_Conflict(i - 1).conf().getLayer())
								Dim currLayerFormat As CNN2DFormat = ConvolutionUtils.getFormatForLayer(layers_Conflict(i).conf().getLayer())
								If preLayerFormat <> currLayerFormat Then
									'NHWC case
									If preLayerFormat = CNN2DFormat.NCHW Then
										input = input.permute(0,3,1,2)
									'NCHW case
									ElseIf preLayerFormat = CNN2DFormat.NHWC Then
										input = input.permute(0,2,3,1)

									Else
										Throw New System.InvalidOperationException("No CNN2DDataFormat type found for previous layer!")
									End If
								End If

								input = layers_Conflict(i).activate(input, train, mgr)
							ElseIf i > 0 AndAlso Convolution1DUtils.hasRnnDataFormat(layers_Conflict(i - 1).conf().getLayer()) AndAlso Convolution1DUtils.hasRnnDataFormat(layers_Conflict(i).conf().getLayer()) Then
								Dim preLayerFormat As RNNFormat = Convolution1DUtils.getRnnFormatFromLayer(layers_Conflict(i - 1).conf().getLayer())
								Dim currLayerFormat As RNNFormat = Convolution1DUtils.getRnnFormatFromLayer(layers_Conflict(i).conf().getLayer())
								'permute for next layer
								If preLayerFormat <> currLayerFormat Then
									input = input.permute(0,2,1)
								End If

								input = layers_Conflict(i).activate(input, train, mgr)


							Else
								input = layers_Conflict(i).activate(input, train, mgr)
							End If
						ElseIf fwdPassType = FwdPassType.RNN_TIMESTEP Then
							'rnnTimeStep case
							If TypeOf layers_Conflict(i) Is RecurrentLayer Then
								input = DirectCast(layers_Conflict(i), RecurrentLayer).rnnTimeStep(reshapeTimeStepInput(input), mgr)
							ElseIf TypeOf layers_Conflict(i) Is BaseWrapperLayer AndAlso TypeOf (DirectCast(layers_Conflict(i), BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
								Dim rl As RecurrentLayer = (DirectCast(DirectCast(layers_Conflict(i), BaseWrapperLayer).getUnderlying(), RecurrentLayer))
								input = rl.rnnTimeStep(reshapeTimeStepInput(input), mgr)
							ElseIf TypeOf layers_Conflict(i) Is MultiLayerNetwork Then
								input = DirectCast(layers_Conflict(i), MultiLayerNetwork).rnnTimeStep(reshapeTimeStepInput(input))
							Else
								input = layers_Conflict(i).activate(input, False, mgr)
							End If
						Else
							Throw New System.ArgumentException("Unsupported forward pass type for this method: " & fwdPassType)
						End If
						layers_Conflict(i).clear()
						'Validation: Exception if invalid (bad layer implementation)
						validateArrayWorkspaces(mgr, input, ArrayType.ACTIVATIONS, i, False, "Output of layer (inference)")

						If wsActCloseNext IsNot Nothing Then
							wsActCloseNext.close()
						End If
						wsActCloseNext = temp
						temp = Nothing
					End Using

					If traceLog Then
						log.trace("Completed forward pass: {} - {}", i, layers_Conflict(i).GetType().Name)
					End If

					'Edge case: for first layer with dropout, inputs can't be in previous workspace (as it hasn't been opened yet)
					'Hence: put inputs in working memory -> set back to default for next use of workspace mgr
					If i = 0 AndAlso wsm <> WorkspaceMode.NONE Then
						mgr.setWorkspace(ArrayType.INPUT, WS_LAYER_ACT_2, WS_LAYER_ACT_X_CONFIG) 'Inputs should always be in the previous WS
					End If
				Next i
			Catch t2 As Exception
				t = t2
			Finally
				If wsActCloseNext IsNot Nothing Then
					Try
						wsActCloseNext.close()
					Catch t2 As Exception
						If t IsNot Nothing Then
							log.error("Encountered second exception while trying to close workspace after initial exception")
							log.error("Original exception:", t)
							Throw t2
						End If
					End Try
				End If
				If temp IsNot Nothing Then
					'Should only be non-null on exception
					Do While temp.ScopeActive
						'For safety, should never occur in theory: a single close() call may not be sufficient, if
						' workspace scope was borrowed and not properly closed when exception occurred
						Try
							temp.close()
						Catch t2 As Exception
							If t IsNot Nothing Then
								log.error("Encountered second exception while trying to close workspace after initial exception")
								log.error("Original exception:", t)
								Throw t2
							End If
						End Try
					Loop
				End If

				Nd4j.MemoryManager.CurrentWorkspace = initialWorkspace

				If t IsNot Nothing Then
					If TypeOf t Is Exception Then
						Throw (CType(t, Exception))
					End If
					Throw New Exception("Error during neural network forward pass", t)
				End If

				If outputWorkspace Is Nothing OrElse TypeOf outputWorkspace Is DummyWorkspace Then
					WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active at the end of outputOfLayerDetached", True)
				Else
					Preconditions.checkState(outputWorkspace.ScopeActive, "Expected output workspace to still be open" & "at end of outputOfLayerDetached, but it is closed. This suggests an implementation or layer workspace problem")
				End If
			End Try

			Return input
		End Function

		Private Function reshapeTimeStepInput(ByVal input As INDArray) As INDArray
			If input.rank() = 2 Then ' dynamically reshape to 3D input with one time-step.
				Dim inShape() As Long = input.shape()
				input = input.reshape(ChrW(inShape(0)), inShape(1), 1)
			End If
			Return input
		End Function

		''' <summary>
		''' Compute activations of all layers from input (inclusive) to output of the final/output layer.
		''' Equivalent to calling <seealso cref="feedForward(Boolean)"/> with train=false
		''' </summary>
		''' <returns> the list of activations for each layer, including the input </returns>
		Public Overridable Function feedForward() As IList(Of INDArray)
			Return feedForward(False)
		End Function

		''' <summary>
		''' Compute activations of all layers from input (inclusive) to output of the final/output layer.
		''' Equivalent to calling <seealso cref="feedForward(INDArray, Boolean)"/> with train = false
		''' </summary>
		''' <returns> the list of activations for each layer, including the input </returns>
		Public Overridable Function feedForward(ByVal input As INDArray) As IList(Of INDArray)
			If input Is Nothing Then
				Throw New System.InvalidOperationException("Unable to perform feed forward; no input found")
			End If
			Me.Input = input
			Return feedForward()
		End Function

		''' <summary>
		''' Compute the activations from the input to the output layer, given mask arrays (that may be null)
		''' The masking arrays are used in situations such an one-to-many and many-to-one rucerrent neural network (RNN)
		''' designs, as well as for supporting time series of varying lengths within the same minibatch for RNNs.
		''' Other than mask arrays, this is equivalent to calling <seealso cref="feedForward(INDArray, Boolean)"/> with train = false
		''' </summary>
		Public Overridable Function feedForward(ByVal input As INDArray, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray) As IList(Of INDArray)
			setLayerMaskArrays(featuresMask, labelsMask)
			Dim list As IList(Of INDArray) = feedForward(input)
			clearLayerMaskArrays()
			Return list
		End Function


		Public Overridable Function gradient() As Gradient Implements org.deeplearning4j.nn.api.Model.gradient
			Return gradient_Conflict
		End Function

		Public Overridable Function gradientAndScore() As Pair(Of Gradient, Double) Implements org.deeplearning4j.nn.api.Model.gradientAndScore
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function


		''' <summary>
		''' Clone the MultiLayerNetwork </summary>
		''' <returns> A cloned MultiLayerNetwork with a copy of the configuration, parameters and updater identical to the current network. </returns>
		Public Overrides Function clone() As MultiLayerNetwork
			If Not initCalled_Conflict Then
				init()
			End If
			Dim conf As MultiLayerConfiguration = Me.layerWiseConfigurations_Conflict.clone()
			Dim ret As New MultiLayerNetwork(conf)
			ret.init(Me.params().dup(), False)

			If solver IsNot Nothing Then
				'If  solver is null: updater hasn't been initialized -> getUpdater call will force initialization, however
				Dim u As Updater = Me.Updater
				Dim updaterState As INDArray = u.StateViewArray
				If updaterState IsNot Nothing Then
					ret.Updater.setStateViewArray(ret, updaterState.dup(), False)
				End If
			End If

			If hasAFrozenLayer() Then
				'correct layers to frozen layers
				Dim clonedLayers() As Layer = ret.Layers
				For i As Integer = 0 To layers_Conflict.Length - 1
					If TypeOf layers_Conflict(i) Is FrozenLayer Then
						clonedLayers(i) = New FrozenLayer(ret.getLayer(i))
					End If
				Next i
				ret.Layers = clonedLayers
			End If
			Return ret
		End Function

		Protected Friend Overridable Function hasAFrozenLayer() As Boolean
			For i As Integer = 0 To layers_Conflict.Length - 2
				If TypeOf layers_Conflict(i) Is FrozenLayer Then
					Return True
				End If
			Next i
			Return False
		End Function


		''' @deprecated To be removed. Use <seealso cref="params()"/> instead 
		<Obsolete("To be removed. Use <seealso cref=""params()""/> instead")>
		Public Overridable Function params(ByVal backwardOnly As Boolean) As INDArray
			Return params()
		End Function


		''' <summary>
		''' Returns a 1 x m vector where the vector is composed of a flattened vector of all of the parameters in the network.<br>
		''' See <seealso cref="getParam(String)"/> and <seealso cref="paramTable()"/> for a more useful/interpretable representation of the parameters.<br>
		''' Note that the parameter vector is not a copy, and changes to the returned INDArray will impact the network parameters.
		''' </summary>
		''' <returns> the parameters for this neural net </returns>
		Public Overridable Function params() As INDArray Implements org.deeplearning4j.nn.api.Model.params, NeuralNetwork.params
			Return flattenedParams
		End Function

		''' <summary>
		''' Set the parameters for this model.
		''' This expects a linear ndarray which then be unpacked internally relative to the expected ordering of the model.<br>
		''' See also: <seealso cref="setParamTable(Map)"/> and <seealso cref="setParam(String, INDArray)"/>
		''' </summary>
		''' <param name="params"> the parameters for the model </param>
		Public Overridable WriteOnly Property Params Implements org.deeplearning4j.nn.api.Model.setParams As INDArray
			Set(ByVal params As INDArray)
				If flattenedParams Is params Then
					Return 'No op
				End If
    
				If flattenedParams IsNot Nothing AndAlso params.length() = flattenedParams.length() Then
					If params IsNot flattenedParams Then
						flattenedParams.assign(params)
					End If
				Else
					If flattenedParams Is Nothing Then
						flattenedParams = params.dup()
					End If
					Dim idx As Integer = 0
					Dim i As Integer = 0
					Do While i < Layers.Length
						Dim layer As Layer = getLayer(i)
						Dim range As Long = layer.numParams()
						If range <= 0 Then
							i += 1
							Continue Do 'Some layers: no parameters (subsampling, etc)
						End If
						Dim get As INDArray = params.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(idx, range + idx))
						layer.Params = get
						idx += range
						i += 1
					Loop
				End If
			End Set
		End Property

		Public Overridable WriteOnly Property ParamsViewArray Implements org.deeplearning4j.nn.api.Model.setParamsViewArray As INDArray
			Set(ByVal params As INDArray)
				Throw New System.NotSupportedException("Not yet implemented")
			End Set
		End Property

		Public Overridable ReadOnly Property GradientsViewArray As INDArray Implements org.deeplearning4j.nn.api.Model.getGradientsViewArray
			Get
				Return flattenedGradients
			End Get
		End Property

		Public Overridable WriteOnly Property BackpropGradientsViewArray Implements org.deeplearning4j.nn.api.Model.setBackpropGradientsViewArray As INDArray
			Set(ByVal gradients As INDArray)
				Dim paramsSoFar As Integer = 0
				For Each layer As Layer In layers_Conflict
					If layer.numParams() = 0 Then
						Continue For
					End If
					layer.BackpropGradientsViewArray = gradients.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(paramsSoFar, paramsSoFar + layer.numParams()))
					paramsSoFar += layer.numParams()
				Next layer
			End Set
		End Property

		Public Overridable ReadOnly Property Config As TrainingConfig Implements org.deeplearning4j.nn.api.Trainable.getConfig
			Get
				Throw New System.NotSupportedException("Not supported")
			End Get
		End Property

		''' <summary>
		''' Returns the number of parameters in the network
		''' </summary>
		''' <returns> The number of parameters </returns>
		Public Overridable Function numParams() As Long Implements org.deeplearning4j.nn.api.Model.numParams
			If Not InitCalled Then
				init()
			End If
			Return If(flattenedParams Is Nothing, 0, flattenedParams.length()) 'Maybe nul for 0 params net
		End Function

		''' <summary>
		''' Returns the number of parameters in the network
		''' </summary>
		''' <param name="backwards"> If true: exclude any parameters uned only in unsupervised layerwise training (such as the decoder
		'''                   parameters in an autoencoder) </param>
		''' <returns> The number of parameters </returns>
		Public Overridable Function numParams(ByVal backwards As Boolean) As Long Implements org.deeplearning4j.nn.api.Model.numParams
			Dim length As Integer = 0
			For i As Integer = 0 To layers_Conflict.Length - 1
				length += layers_Conflict(i).numParams(backwards)
			Next i

			Return length
		End Function

		''' <summary>
		''' Sets the input and labels and returns the F1 score for the prediction with respect to the true labels
		''' </summary>
		''' <param name="data"> the data to score </param>
		''' <returns> the score for the given input,label pairs </returns>
		Public Overridable Function f1Score(ByVal data As org.nd4j.linalg.dataset.api.DataSet) As Double Implements Classifier.f1Score
			Return f1Score(data.Features, data.Labels)
		End Function

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
		''' Perform minibatch training on all minibatches in the DataSetIterator for 1 epoch.<br>
		''' Note that this method does not do layerwise  pretraining.<br>
		''' For pretraining use method pretrain.. <seealso cref="pretrain(DataSetIterator)"/><br> </summary>
		''' <param name="iterator"> Training data (DataSetIterator) </param>
		Public Overridable Sub fit(ByVal iterator As DataSetIterator) Implements Classifier.fit, NeuralNetwork.fit
			Try
				fitHelper(iterator)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Sub

		Private Sub fitHelper(ByVal iterator As DataSetIterator)
			SyncLock Me
				' we're wrapping all iterators into AsyncDataSetIterator to provide background prefetch - where appropriate
				Dim iter As DataSetIterator
				Dim destructable As Boolean = False
				If iterator.asyncSupported() Then
					iter = New AsyncDataSetIterator(iterator, Math.Min(Nd4j.AffinityManager.NumberOfDevices * 2, 2), True)
					destructable = True
				Else
					iter = iterator
				End If
        
				For Each tl As TrainingListener In trainingListeners
					tl.onEpochStart(Me)
				Next tl
        
				Dim workspaceMgr As LayerWorkspaceMgr
				If LayerWiseConfigurations.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
					workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
				Else
					workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.UPDATER_WORKING_MEM, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).build()
				End If
				workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)
        
				update(TaskUtils.buildTask(iter))
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If Not iter.hasNext() AndAlso iter.resetSupported() Then
					iter.reset()
				End If
				Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
				Do While iter.MoveNext()
        
					Dim [next] As DataSet = iter.Current
					Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()
        
					lastEtlTime_Conflict.set((time2 - time1))
        
					If [next].Features Is Nothing OrElse [next].Labels Is Nothing Then
						Exit Do
					End If
        
					' TODO: basically we want to wrap internals of this loop into workspace
        
        
					Dim hasMaskArrays As Boolean = [next].hasMaskArrays()
        
					If layerWiseConfigurations_Conflict.getBackpropType() = BackpropType.TruncatedBPTT Then
						doTruncatedBPTT([next].Features, [next].Labels, [next].FeaturesMaskArray, [next].LabelsMaskArray, workspaceMgr)
					Else
						If hasMaskArrays Then
							setLayerMaskArrays([next].FeaturesMaskArray, [next].LabelsMaskArray)
						End If
        
						Input = [next].Features
						Labels = [next].Labels
        
						If solver Is Nothing Then
							Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
								solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
							End Using
						End If
        
						'TODO CACHE
						solver.optimize(workspaceMgr)
					End If
        
					If hasMaskArrays Then
						clearLayerMaskArrays()
					End If
        
					time1 = DateTimeHelper.CurrentUnixTimeMillis()
					synchronizeIterEpochCounts()
				Loop
        
				If trainingListeners.Count > 0 Then
					For Each tl As TrainingListener In trainingListeners
						tl.onEpochEnd(Me)
					Next tl
				End If
        
				clearLayersStates()
        
				If destructable Then
					DirectCast(iter, AsyncDataSetIterator).shutdown()
				End If
        
				incrementEpochCount()
			End SyncLock
		End Sub

		''' <summary>
		''' Calculate parameter gradients and input activation gradients given the input and labels, and optionally mask arrays
		''' </summary>
		''' <param name="features">  Features for gradient calculation </param>
		''' <param name="label">     Labels for gradient </param>
		''' <param name="fMask">     Features mask array (may be null) </param>
		''' <param name="labelMask"> Label mask array (may be null) </param>
		''' <returns> A pair of gradient arrays: parameter gradients (in Gradient object) and input activation gradients </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient,org.nd4j.linalg.api.ndarray.INDArray> calculateGradients(@NonNull INDArray features, @NonNull INDArray label, org.nd4j.linalg.api.ndarray.INDArray fMask, org.nd4j.linalg.api.ndarray.INDArray labelMask)
		Public Overridable Function calculateGradients(ByVal features As INDArray, ByVal label As INDArray, ByVal fMask As INDArray, ByVal labelMask As INDArray) As Pair(Of Gradient, INDArray)
			Try
				Return calculateGradientsHelper(features, label, fMask, labelMask)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Private Function calculateGradientsHelper(ByVal features As INDArray, ByVal label As INDArray, ByVal fMask As INDArray, ByVal labelMask As INDArray) As Pair(Of Gradient, INDArray)
			Input = features
			Labels = label
			setLayerMaskArrays(fMask, labelMask)

			Dim mgr As LayerWorkspaceMgr
			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				mgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				mgr = LayerWorkspaceMgr.builder().with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

				If layerWiseConfigurations_Conflict.getCacheMode() IsNot Nothing Then
					'For now: store cache mode activations in activations workspace
					mgr.setWorkspace(ArrayType.FF_CACHE, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG)
				End If
			End If
			mgr.setHelperWorkspacePointers(helperWorkspaces)

			'Calculate activations (which are stored in each layer, and used in backprop)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				'First: do a feed-forward through the network
				'Note that we don't actually need to do the full forward pass through the output layer right now; but we do
				' need the input to the output layer to be set (such that backprop can be done)
				Dim activations As IList(Of INDArray) = ffToLayerActivationsInWs(layers_Conflict.Length - 2, FwdPassType.STANDARD, False, input_Conflict, mask_Conflict, fMask)
				If trainingListeners.Count > 0 Then
					'TODO: We possibly do want output layer activations in some cases here...
					For Each tl As TrainingListener In trainingListeners
						tl.onForwardPass(Me, activations)
					Next tl
				End If
				Dim inputToOutputLayer As INDArray = activations(activations.Count - 1)
				If layerWiseConfigurations_Conflict.getInputPreProcess(layers_Conflict.Length - 1) IsNot Nothing Then
					inputToOutputLayer = layerWiseConfigurations_Conflict.getInputPreProcess(layers_Conflict.Length - 1).preProcess(inputToOutputLayer, InputMiniBatchSize, mgr)
					'Validate activations location
				End If
				OutputLayer.setInput(inputToOutputLayer, mgr)

				Dim p As Pair(Of Gradient, INDArray) = calcBackpropGradients(Nothing, True, False, True)
				If p.Second IsNot Nothing Then
					p.Second = p.Second.detach()
				End If
				Return p
			End Using
		End Function

		''' <summary>
		''' Calculate gradients and errors. Used in two places:
		''' (a) backprop (for standard multi layer network learning)
		''' (b) backpropGradient (layer method, for when MultiLayerNetwork is used as a layer) </summary>
		''' <param name="epsilon"> Errors (technically errors .* activations). Not used if withOutputLayer = true </param>
		''' <param name="withOutputLayer"> if true: assume last layer is output layer, and calculate errors based on labels. In this
		'''                        case, the epsilon input is not used (may/should be null).
		'''                        If false: calculate backprop gradients </param>
		''' <param name="returnInputActGrad"> If true: terun the input activation gradients (detached). False: don't return </param>
		''' <returns> Gradients and the error (epsilon) at the input </returns>
		Protected Friend Overridable Function calcBackpropGradients(ByVal epsilon As INDArray, ByVal withOutputLayer As Boolean, ByVal tbptt As Boolean, ByVal returnInputActGrad As Boolean) As Pair(Of Gradient, INDArray)
			If flattenedGradients Is Nothing Then
				initGradientsView()
			End If
			Dim multiGradientKey As String
			Dim gradient As Gradient = New DefaultGradient(flattenedGradients)

			Dim mgrEven As LayerWorkspaceMgr
			Dim mgrOdd As LayerWorkspaceMgr

			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				mgrEven = LayerWorkspaceMgr.noWorkspaces()
				mgrOdd = mgrEven
				WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active in calcBackpropGradients when " & "training workspace is set to none")
			Else
	'            
	'            Workspaces for backprop in MLN share some features with outputOfLayerDetached, in terms of the
	'            "two alternating workspaces" idea (but for activation gradients here, instead of activations there).
	'
	'            Workspace design for backprop:
	'            First: we calculate all activations, and ensure they are in WS_ALL_LAYERS_ACT. We assume this is done
	'                   EXTERNALLY to this method
	'            Then: we iterate backwards over layers.
	'
	'            Activations gradient workspaces: opened/closed every second layer.
	'            mgrEven (WS_LAYER_ACT_1) activation grad WS opens at start of 8, 4, 2, 0; closed at end of 7, 5, 3, 1 etc
	'            mgrOdd (WS_LAYER_ACT_2) activation grad WS opens at start of 7, 3, 5, 1; closed at end of 6, 4, 2, 0 etc
	'
	'             

				mgrEven = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATION_GRAD, WS_LAYER_ACT_1, WS_LAYER_ACT_X_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

				mgrOdd = LayerWorkspaceMgr.builder().with(ArrayType.ACTIVATIONS, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATION_GRAD, WS_LAYER_ACT_2, WS_LAYER_ACT_X_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

				If epsilon Is Nothing Then
					'If epsilon is non-null: external errors use case -> inputs are already detached
					WorkspaceUtils.assertOpenActiveAndCurrent(WS_ALL_LAYERS_ACT, "calcBackpropGradients method requires workspace WS_ALL_LAYERS_ACT" & " to be open when workspaces are used")
				End If
			End If
			mgrEven.setHelperWorkspacePointers(helperWorkspaces)
			mgrOdd.setHelperWorkspacePointers(helperWorkspaces)

			'calculate and apply the backward gradient for every layer
	'        
	'         * Skip the output layer for the indexing and just loop backwards updating the coefficients for each layer.
	'         * (when withOutputLayer == true)
	'         *
	'         * Activate applies the activation function for each layer and sets that as the input for the following layer.
	'         *
	'         * Typical literature contains most trivial case for the error calculation: wT * weights
	'         * This interpretation transpose a few things to get mini batch because ND4J is rows vs columns organization for params
	'         
			Dim numLayers As Integer = getnLayers()
			'Store gradients is a list; used to ensure iteration order in DefaultGradient linked hash map. i.e., layer 0 first instead of output layer
			Dim gradientList As New LinkedList(Of Triple(Of String, INDArray, Char))()


			Dim currPair As Pair(Of Gradient, INDArray) = Nothing
			Dim wsActGradCloseNext As MemoryWorkspace = Nothing
			Dim wsActGradTemp As MemoryWorkspace = Nothing
			Dim initialWorkspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			Dim traceLog As Boolean = log.isTraceEnabled()

			Dim t As Exception = Nothing
			Try
				For i As Integer = layers_Conflict.Length - 1 To 0 Step -1
					If TypeOf layers_Conflict(i) Is FrozenLayer Then
						Exit For
					End If

					If traceLog Then
						log.trace("About to backprop: {} - {}", i, layers_Conflict(i).GetType().Name)
					End If

					Dim workspaceMgr As LayerWorkspaceMgr = (If(i Mod 2 = 0, mgrEven, mgrOdd))

					If withOutputLayer AndAlso i = layers_Conflict.Length - 1 Then
						If Not (TypeOf OutputLayer Is IOutputLayer) Then
							log.warn("Warning: final layer isn't output layer. You cannot use backprop without an output layer.")
							Return Nothing
						End If

						Dim outputLayer As IOutputLayer = DirectCast(Me.OutputLayer, IOutputLayer)
						If labels_Conflict Is Nothing AndAlso outputLayer.needsLabels() Then
							Throw New System.InvalidOperationException("No labels found")
						End If
						outputLayer.Labels = labels_Conflict
					End If

					'Open activation gradients WS *then* BP working memory, so BP working memory is opened last for use in layers
					wsActGradTemp = workspaceMgr.notifyScopeEntered(ArrayType.ACTIVATION_GRAD)
					Using wsBPWorking As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.BP_WORKING_MEM)

						'Note that because we're opening activation workspaces not in a simple nested order, we'll manually
						' override the previous workspace setting. Otherwise, when we close these workspaces, the "current"
						' workspace may be set to the incorrect one
						wsActGradTemp.PreviousWorkspace = initialWorkspace
						wsBPWorking.PreviousWorkspace = initialWorkspace

						Dim eps As INDArray = (If(i = layers_Conflict.Length - 1, epsilon, currPair.Right)) 'eps is null for OutputLayer

						If Not tbptt Then
							'Standard case
							currPair = layers_Conflict(i).backpropGradient(eps, workspaceMgr)
						Else
							'TBPTT gradient
							If TypeOf layers_Conflict(i) Is RecurrentLayer Then
								currPair = DirectCast(layers_Conflict(i), RecurrentLayer).tbpttBackpropGradient(currPair.Second, layerWiseConfigurations_Conflict.getTbpttBackLength(), workspaceMgr)
							Else
								currPair = layers_Conflict(i).backpropGradient(currPair.Second, workspaceMgr)
							End If
						End If

						If currPair.Second IsNot Nothing Then
							'Edge case: may be null for Embedding layer, for example
							validateArrayWorkspaces(workspaceMgr, currPair.Second, ArrayType.ACTIVATION_GRAD, i, False, "Backprop")
						End If

						For Each entry As KeyValuePair(Of String, INDArray) In currPair.First.gradientForVariable().SetOfKeyValuePairs()
							Dim origName As String = entry.Key
							multiGradientKey = i.ToString() & "_" & origName
							gradientList.AddLast(New Triple(Of )(multiGradientKey, entry.Value, currPair.First.flatteningOrderForVariable(origName)))
						Next entry
						If LayerWiseConfigurations.getInputPreProcess(i) IsNot Nothing Then
							currPair = New Pair(Of Gradient, INDArray)(currPair.First, Me.layerWiseConfigurations_Conflict.getInputPreProcess(i).backprop(currPair.Second, InputMiniBatchSize, workspaceMgr))
							If i > 0 AndAlso currPair.Second IsNot Nothing Then
								validateArrayWorkspaces(workspaceMgr, currPair.Second, ArrayType.ACTIVATION_GRAD, i, True, "Backprop")
							End If
						End If

						If i = 0 Then
							If returnInputActGrad AndAlso currPair.Second IsNot Nothing Then
								currPair.Second = currPair.Second.detach()
							Else
								currPair.Second = Nothing
							End If
						End If

						If wsActGradCloseNext IsNot Nothing Then
							wsActGradCloseNext.close()
						End If
						wsActGradCloseNext = wsActGradTemp
						wsActGradTemp = Nothing
					End Using

					If traceLog Then
						log.trace("Completed backprop: {} - {}", i, layers_Conflict(i).GetType().Name)
					End If
				Next i
			Catch thr As Exception
				t = thr
			Finally
				If wsActGradCloseNext IsNot Nothing Then
					Try
						wsActGradCloseNext.close()
					Catch t2 As Exception
						If t IsNot Nothing Then
							log.error("Encountered second exception while trying to close workspace after initial exception")
							log.error("Original exception:", t)
							Throw t2
						End If
					End Try
				End If
				If wsActGradTemp IsNot Nothing Then
					'Should only be non-null on exception
					Try
						wsActGradTemp.close()
					Catch t2 As Exception
						If t IsNot Nothing Then
							log.error("Encountered second exception while trying to close workspace after initial exception")
							log.error("Original exception:", t)
							Throw t2
						End If
					End Try
				End If
				Nd4j.MemoryManager.CurrentWorkspace = initialWorkspace

				If t IsNot Nothing Then
					If TypeOf t Is Exception Then
						Throw (CType(t, Exception))
					End If
					Throw New Exception("Error during neural network forward pass", t)
				End If
			End Try

			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				WorkspaceUtils.assertNoWorkspacesOpen("Expected no workspace active in calcBackpropGradients when " & "training workspace is set to none")
			Else
				If epsilon Is Nothing Then
					'If epsilon != null: external errors use case (inputs are detached instead)
					WorkspaceUtils.assertOpenActiveAndCurrent(WS_ALL_LAYERS_ACT, "calcBackpropGradients: WS_ALL_LAYERS_ACT is no" & " longer the currently open/active workspace")
				End If
			End If

			'Add gradients to Gradients (map), in correct order
			For Each triple As Triple(Of String, INDArray, Char) In gradientList
				gradient.setGradientFor(triple.getFirst(), triple.getSecond(), triple.getThird())
			Next triple

			Return New Pair(Of Gradient, INDArray)(gradient, currPair.Second)
		End Function

		Protected Friend Overridable Sub doTruncatedBPTT(ByVal input As INDArray, ByVal labels As INDArray, ByVal featuresMaskArray As INDArray, ByVal labelsMaskArray As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			If input.rank() <> 3 OrElse labels.rank() <> 3 Then
				log.warn("Cannot do truncated BPTT with non-3d inputs or labels. Expect input with shape [miniBatchSize,nIn,timeSeriesLength], got " & java.util.Arrays.toString(input.shape()) & vbTab & "and labels with shape " & java.util.Arrays.toString(labels.shape()))
				Return
			End If
			If input.size(2) <> labels.size(2) Then
				log.warn("Input and label time series have different lengths: {} input length, {} label length", input.size(2), labels.size(2))
				Return
			End If

			Dim fwdLen As Integer = layerWiseConfigurations_Conflict.getTbpttFwdLength()
			update(TaskUtils.buildTask(input, labels))
			Dim timeSeriesLength As val = input.size(2)
			Dim nSubsets As Long = timeSeriesLength / fwdLen
			If timeSeriesLength Mod fwdLen <> 0 Then
				nSubsets += 1 'Example: 100 fwdLen with timeSeriesLength=120 -> want 2 subsets (1 of size 100, 1 of size 20)
			End If

			rnnClearPreviousState()

			For i As Integer = 0 To nSubsets - 1
				Dim startTimeIdx As Long = i * fwdLen
				Dim endTimeIdx As Long = startTimeIdx + fwdLen
				If endTimeIdx > timeSeriesLength Then
					endTimeIdx = timeSeriesLength
				End If

				If startTimeIdx > Integer.MaxValue OrElse endTimeIdx > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				Dim subsets() As INDArray = getSubsetsForTbptt(CInt(startTimeIdx), CInt(endTimeIdx), input, labels, featuresMaskArray, labelsMaskArray)

				Me.Input = subsets(0)
				Me.Labels = subsets(1)
				setLayerMaskArrays(subsets(2), subsets(3))

				If solver Is Nothing Then
					Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
					End Using
				End If
				solver.optimize(workspaceMgr)

				'Finally, update the state of the RNN layers:
				updateRnnStateWithTBPTTState()
			Next i

			rnnClearPreviousState()
			clearLayerMaskArrays()
		End Sub

		Private Function getSubsetsForTbptt(ByVal startTimeIdx As Integer, ByVal endTimeIdx As Integer, ByVal input As INDArray, ByVal labels As INDArray, ByVal fMask As INDArray, ByVal lMask As INDArray) As INDArray()
			Dim [out](3) As INDArray
			[out](0) = input.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
			[out](1) = labels.get(NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))

			If fMask IsNot Nothing Then
				[out](2) = fMask.get(NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
			End If
			If lMask IsNot Nothing Then
				[out](3) = lMask.get(NDArrayIndex.all(), NDArrayIndex.interval(startTimeIdx, endTimeIdx))
			End If

			Return [out]
		End Function

		''' <summary>
		''' Intended for internal/developer use
		''' </summary>
		Public Overridable Sub updateRnnStateWithTBPTTState()
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
		''' Get the <seealso cref="TrainingListener"/>s set for this network, if any </summary>
		''' <returns> listeners set for this network </returns>
		Public Overridable Property Listeners As ICollection(Of TrainingListener)
			Get
				Return trainingListeners
			End Get
			Set(ByVal listeners As ICollection(Of TrainingListener))
				If layers_Conflict Is Nothing Then
					init()
				End If
				For Each layer As Layer In layers_Conflict
					layer.setListeners(listeners)
				Next layer
    
				If solver IsNot Nothing Then
					solver.Listeners = listeners
				End If
    
				Me.trainingListeners.Clear()
				If listeners IsNot Nothing Then
					Me.trainingListeners.addAll(listeners)
				End If
			End Set
		End Property

		''' @deprecated Use <seealso cref="getListeners()"/> 
		<Obsolete("Use <seealso cref=""getListeners()""/>")>
		Public Overridable ReadOnly Property TrainingListeners As ICollection(Of TrainingListener)
			Get
				Return trainingListeners
			End Get
		End Property


		''' <summary>
		''' This method ADDS additional TrainingListener to existing listeners
		''' </summary>
		''' <param name="listeners"> </param>
		Public Overridable Sub addListeners(ParamArray ByVal listeners() As TrainingListener) Implements org.deeplearning4j.nn.api.Model.addListeners
			Collections.addAll(trainingListeners, listeners)

			' fixme this is wrong, since it removes existing listeners from the solver
			If solver IsNot Nothing Then
				solver.Listeners = Me.trainingListeners
			End If
		End Sub

		Public Overridable WriteOnly Property Listeners Implements org.deeplearning4j.nn.api.Model.setListeners, Layer.setListeners As TrainingListener()
			Set(ByVal listeners() As TrainingListener)
				Dim cListeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()
				'Check: user might have done setListeners(null) thinking this would clear the current listeners.
				'This results in an TrainingListener[1] with a single null value -> results in a NPE later
				If listeners IsNot Nothing AndAlso listeners.Length > 0 Then
					For Each i As TrainingListener In listeners
						If i IsNot Nothing Then
							cListeners.Add(i)
						End If
					Next i
				End If
				setListeners(cListeners)
			End Set
		End Property

		''' <summary>
		''' Usable only for classification networks in conjunction with OutputLayer. Cannot be used with RnnOutputLayer,
		''' CnnLossLayer, or networks used for regression.<br>
		''' To get the raw output activations of the output layer, use <seealso cref="output(INDArray)"/> or similar.<br>
		''' <br>
		''' Equivalent to argmax(this.output(input)): Returns the predicted class indices corresponding to the predictions
		''' for each example in the features array.
		''' </summary>
		''' <param name="d"> The input features to perform inference on </param>
		''' <returns> The predicted class index for each example </returns>
		Public Overridable Function predict(ByVal d As INDArray) As Integer() Implements Classifier.predict
			Dim output As INDArray = Me.output(d, TrainingMode.TEST)

			If d.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If

			Preconditions.checkState(output.rank() = 2, "predict(INDArray) method can only be used on rank 2 output - got array with rank %s", output.rank())
			Return output.argMax(1).toIntVector()
		End Function

		''' <summary>
		''' As per <seealso cref="predict(INDArray)"/> but the returned values are looked up from the list of label names
		''' in the provided DataSet
		''' </summary>
		Public Overridable Function predict(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As IList(Of String)
			Preconditions.checkState(dataSet.getLabelNamesList() IsNot Nothing, "This method can only be used when the DataSet contains a label name list")
			Dim intRet() As Integer = predict(dataSet.Features)
			Dim ret As IList(Of String) = New List(Of String)()
			For i As Integer = 0 To intRet.Length - 1
				ret.Insert(i, dataSet.getLabelName(intRet(i)))
			Next i
			Return ret
		End Function

		''' <summary>
		''' Fit the model for one iteration on the provided data
		''' </summary>
		''' <param name="data">   the examples to classify (one example in each row) </param>
		''' <param name="labels"> the example labels(a binary outcome matrix) </param>
		Public Overridable Sub fit(ByVal data As INDArray, ByVal labels As INDArray) Implements Classifier.fit
			fit(data, labels, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Fit the model for one iteration on the provided data
		''' </summary>
		''' <param name="features">   the examples to classify (one example in each row) </param>
		''' <param name="labels"> the example labels(a binary outcome matrix) </param>
		''' <param name="featuresMask"> The mask array for the features (used for variable length time series, etc). May be null. </param>
		''' <param name="labelsMask"> The mask array for the labels (used for variable length time series, etc). May be null. </param>
		Public Overridable Sub fit(ByVal features As INDArray, ByVal labels As INDArray, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray)
			SyncLock Me
				Try
					fitHelper(features, labels, featuresMask, labelsMask)
				Catch e As System.OutOfMemoryException
					CrashReportingUtil.writeMemoryCrashDump(Me, e)
					Throw e
				End Try
			End SyncLock
		End Sub

		Private Sub fitHelper(ByVal features As INDArray, ByVal labels As INDArray, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray)
			If numParams() = 0 Then
				'No op: can't fit a network with 0 parameters
				Return
			End If

			Input = features
			Me.Labels = labels
			Me.setLayerMaskArrays(featuresMask, labelsMask)
			update(TaskUtils.buildTask(features, labels))

			Dim workspaceMgr As LayerWorkspaceMgr
			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() Is Nothing Then
				workspaceMgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				workspaceMgr = LayerWorkspaceMgr.builder().with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.UPDATER_WORKING_MEM, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).build()
			End If
			workspaceMgr.setHelperWorkspacePointers(helperWorkspaces)

			If layerWiseConfigurations_Conflict.getBackpropType() = BackpropType.TruncatedBPTT Then
				doTruncatedBPTT(features, labels, featuresMask, labelsMask, workspaceMgr)
			Else
				If solver Is Nothing Then
					Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
					End Using
				End If
				'TODO CACHE WORKSPACE, IF USED???
				solver.optimize(workspaceMgr)
			End If

			clearLayerMaskArrays()
			clearLayersStates()
			synchronizeIterEpochCounts()
		End Sub

		Public Overridable Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) Implements org.deeplearning4j.nn.api.Model.fit
			Throw New System.NotSupportedException("Not supported: use pretrainLayer")
		End Sub


		''' <summary>
		''' Fit the model for one iteration on the provided data
		''' </summary>
		''' <param name="data"> the data to train on </param>
		Public Overridable Sub fit(ByVal data As org.nd4j.linalg.dataset.api.DataSet) Implements Classifier.fit, NeuralNetwork.fit
			fit(data.Features, data.Labels, data.FeaturesMaskArray, data.LabelsMaskArray)
		End Sub

		''' <summary>
		''' Fit the model for one iteration on the provided data
		''' </summary>
		''' <param name="examples"> the examples to classify (one example in each row) </param>
		''' <param name="labels">   the labels for each example (the number of labels must match </param>
		Public Overridable Sub fit(ByVal examples As INDArray, ByVal labels() As Integer) Implements Classifier.fit
			Dim layerConf As org.deeplearning4j.nn.conf.layers.OutputLayer = CType(OutputLayer.conf().getLayer(), org.deeplearning4j.nn.conf.layers.OutputLayer)

			If layerConf.getNOut() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			fit(examples, FeatureUtil.toOutcomeMatrix(labels, CInt(Math.Truncate(layerConf.getNOut()))))
		End Sub


		''' <summary>
		''' Perform inference on the provided input/features - i.e., perform forward pass using the provided input/features
		''' and return the output of the final layer.
		''' </summary>
		''' <param name="input"> Input to the network </param>
		''' <param name="train"> whether the output is test or train. This mainly affect hyper parameters such as dropout and
		'''              batch normalization, which have different behaviour for test vs. train </param>
		''' <returns> The network predictions - i.e., the activations of the final layer </returns>
		Public Overridable Function output(ByVal input As INDArray, ByVal train As TrainingMode) As INDArray
			Return output(input, train = TrainingMode.TRAIN)
		End Function

		''' <summary>
		''' Perform inference on the provided input/features - i.e., perform forward pass using the provided input/features
		''' and return the output of the final layer.
		''' </summary>
		''' <param name="input"> Input to the network </param>
		''' <param name="train"> whether the output is test or train. This mainly affect hyper parameters such as dropout and
		'''              batch normalization, which have different behaviour for test vs. train </param>
		''' <returns> The network predictions - i.e., the activations of the final layer </returns>
		Public Overridable Function output(ByVal input As INDArray, ByVal train As Boolean) As INDArray
			Return output(input, train, Nothing, Nothing)
		End Function

		''' <summary>
		''' Calculate the output of the network, with masking arrays. The masking arrays are used in situations such
		''' as one-to-many and many-to-one recurrent neural network (RNN) designs, as well as for supporting time series
		''' of varying lengths within the same minibatch.
		''' </summary>
		Public Overridable Function output(ByVal input As INDArray, ByVal train As Boolean, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray) As INDArray
			Return output(input, train, featuresMask, labelsMask, Nothing)
		End Function

		''' <summary>
		''' Get the network output, which is optionally placed in the specified memory workspace.<br>
		''' If no memory workspace is provided, the output will be detached (not in any workspace).<br>
		''' If a memory workspace is provided, the output activation array (i.e., the INDArray returned by this method)
		''' will be placed in the specified workspace. This workspace must be opened by the user before calling this method -
		''' and the user is responsible for (a) closing this workspace, and (b) ensuring the output array is not used out
		''' of scope (i.e., not used after closing the workspace to which it belongs - as this is likely to cause either
		''' an exception when used, or a crash).
		''' </summary>
		''' <param name="input">           Input to the network </param>
		''' <param name="train">           True for train, false otherwise </param>
		''' <param name="outputWorkspace"> May be null. If not null: the workspace MUST be opened before calling this method. </param>
		''' <returns> The output/activations from the network (either detached or in the specified workspace if provided) </returns>
		Public Overridable Function output(ByVal input As INDArray, ByVal train As Boolean, ByVal outputWorkspace As MemoryWorkspace) As INDArray
			Return output(input, train, Nothing, Nothing, outputWorkspace)
		End Function

		''' <summary>
		''' Get the network output, which is optionally placed in the specified memory workspace.<br>
		''' If no memory workspace is provided, the output will be detached (not in any workspace).<br>
		''' If a memory workspace is provided, the output activation array (i.e., the INDArray returned by this method)
		''' will be placed in the specified workspace. This workspace must be opened by the user before calling this method -
		''' and the user is responsible for (a) closing this workspace, and (b) ensuring the output array is not used out
		''' of scope (i.e., not used after closing the workspace to which it belongs - as this is likely to cause either
		''' an exception when used, or a crash).
		''' </summary>
		''' <param name="input">           Input to the network </param>
		''' <param name="train">           True for train, false otherwise </param>
		''' <param name="outputWorkspace"> May be null. If not null: the workspace MUST be opened before calling this method. </param>
		''' <returns> The output/activations from the network (either detached or in the specified workspace if provided) </returns>
		Public Overridable Function output(ByVal input As INDArray, ByVal train As Boolean, ByVal featuresMask As INDArray, ByVal labelsMask As INDArray, ByVal outputWorkspace As MemoryWorkspace) As INDArray
			SyncLock Me
				Try
					Return outputOfLayerDetached(train, FwdPassType.STANDARD, layers_Conflict.Length - 1, input, featuresMask, labelsMask, outputWorkspace)
				Catch e As System.OutOfMemoryException
					CrashReportingUtil.writeMemoryCrashDump(Me, e)
					Throw e
				End Try
			End SyncLock
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
'ORIGINAL LINE: public synchronized <T> T output(@NonNull INDArray inputs, org.nd4j.linalg.api.ndarray.INDArray inputMasks, org.nd4j.linalg.api.ndarray.INDArray labelMasks, @NonNull OutputAdapter<T> outputAdapter)
		Public Overridable Function output(Of T)(ByVal inputs As INDArray, ByVal inputMasks As INDArray, ByVal labelMasks As INDArray, ByVal outputAdapter As OutputAdapter(Of T)) As T
			SyncLock Me
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(WS_ALL_LAYERS_ACT_CONFIG, WS_OUTPUT_MEM)
					If TypeOf outputAdapter Is ModelAdapter Then
						Return CType(outputAdapter, ModelAdapter(Of T)).apply(Me, New INDArray(){inputs}, New INDArray(){ inputMasks}, New INDArray(){labelMasks})
					Else
						Return outputAdapter.apply(output(inputs, False, inputMasks, labelMasks, ws))
					End If
				End Using
			End SyncLock
		End Function

		''' <summary>
		''' Perform inference on the provided input/features - i.e., perform forward pass using the provided input/features
		''' and return the output of the final layer. Equivalent to <seealso cref="output(INDArray, Boolean)"/> with train=false - i.e.,
		''' this method is used for inference.
		''' </summary>
		''' <param name="input"> Input to the network </param>
		''' <returns> The network predictions - i.e., the activations of the final layer </returns>
		Public Overridable Function output(ByVal input As INDArray) As INDArray
			Return output(input, TrainingMode.TEST)
		End Function

		''' <summary>
		''' Generate the output for all examples/batches in the input iterator, and concatenate them into a single array.
		''' See <seealso cref="output(INDArray)"/><br>
		''' NOTE 1: The output array can require a considerable amount of memory for iterators with a large number of examples<br>
		''' NOTE 2: This method cannot be used for variable length time series outputs, as this would require padding arrays
		''' for some outputs, or returning a mask array (which cannot be done with this method). For variable length time
		''' series applications, use one of the other output methods. This method also cannot be used with fully convolutional
		''' networks with different output sizes (for example, segmentation on different input image sizes).
		''' 
		''' </summary>
		''' <param name="iterator"> Data to pass through the network </param>
		''' <returns> output for all examples in the iterator, concatenated into a </returns>
		Public Overridable Function output(ByVal iterator As DataSetIterator, ByVal train As Boolean) As INDArray
			Dim outList As IList(Of INDArray) = New List(Of INDArray)()
			Dim firstOutputShape() As Long = Nothing
			Do While iterator.MoveNext()
				Dim [next] As DataSet = iterator.Current
				Dim features As INDArray = [next].Features

				If features Is Nothing Then
					Continue Do
				End If

				Dim fMask As INDArray = [next].FeaturesMaskArray
				Dim lMask As INDArray = [next].LabelsMaskArray
'JAVA TO VB CONVERTER NOTE: The local variable output was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
				Dim output_Conflict As INDArray = Me.output(features, train, fMask, lMask)
				outList.Add(output_Conflict)
				If firstOutputShape Is Nothing Then
					firstOutputShape = output_Conflict.shape()
				Else
					'Validate that shapes are the same (may not be, for some RNN variable length time series applications)
					Dim currShape() As Long = output_Conflict.shape()
					Preconditions.checkState(firstOutputShape.Length = currShape.Length, "Error during forward pass:" & "different minibatches have different output array ranks - first minibatch shape %s, last minibatch shape %s", firstOutputShape, currShape)
					For i As Integer = 1 To currShape.Length - 1 'Skip checking minibatch dimension, fine if this varies
						Preconditions.checkState(firstOutputShape(i) = currShape(i), "Current output shape does not match first" & " output array shape at position %s: all dimensions must match other than the first dimension." & vbLf & " For variable length output size/length use cases such as for RNNs with multiple sequence lengths," & " use one of the other (non iterator) output methods. First batch output shape: %s, current batch output shape: %s", i, firstOutputShape, currShape)
					Next i
				End If
			Loop
			Return Nd4j.concat(0, CType(outList, List(Of INDArray)).ToArray())
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="output(DataSetIterator, Boolean)"/> with train=false
		''' </summary>
		Public Overridable Function output(ByVal iterator As DataSetIterator) As INDArray
			Return output(iterator, False)
		End Function

		''' <summary>
		''' Perform inference and then calculate the F1 score of the output(input) vs. the labels.
		''' </summary>
		''' <param name="input">  the input to perform inference with </param>
		''' <param name="labels"> the true labels </param>
		''' <returns> the score for the given input,label pairs </returns>
		Public Overridable Function f1Score(ByVal input As INDArray, ByVal labels As INDArray) As Double Implements Classifier.f1Score
			feedForward(input)
			Me.Labels = labels
			Dim eval As New Evaluation()
			eval.eval(labels, output(input))
			Return eval.f1()
		End Function

		''' @deprecated Will be removed in a future release 
		<Obsolete("Will be removed in a future release")>
		Public Overridable Function numLabels() As Integer Implements Classifier.numLabels
			Return CInt(labels_Conflict.size(1))
		End Function

		''' <summary>
		''' Sets the input and labels and calculates the score (value of the output layer loss function plus l1/l2 if applicable)
		''' for the prediction with respect to the true labels<br>
		''' This is equivalent to <seealso cref="score(DataSet, Boolean)"/> with training==false. </summary>
		''' <param name="data"> the data to score </param>
		''' <returns> the score for the given input,label pairs </returns>
		''' <seealso cref= #score(DataSet, boolean) </seealso>
		Public Overridable Function score(ByVal data As DataSet) As Double
			Return score(data, False)
		End Function

		''' <summary>
		''' Sets the input and labels and calculates the score (value of the output layer loss function plus l1/l2 if applicable)
		''' for the prediction with respect to the true labels<br> </summary>
		''' <param name="data"> data to calculate score for </param>
		''' <param name="training"> If true: score during training. If false: score at test time. This can affect the application of
		'''                 certain features, such as dropout and dropconnect (which are applied at training time only) </param>
		''' <returns> the score (value of the loss function) </returns>
		Public Overridable Function score(ByVal data As DataSet, ByVal training As Boolean) As Double
			Try
				Return scoreHelper(data, training)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Private Function scoreHelper(ByVal data As DataSet, ByVal training As Boolean) As Double
			Dim hasMaskArray As Boolean = data.hasMaskArrays()
			If hasMaskArray Then
				setLayerMaskArrays(data.FeaturesMaskArray, data.LabelsMaskArray)
			End If

			If Not (TypeOf OutputLayer Is IOutputLayer) Then
				Throw New System.InvalidOperationException("Cannot calculate score if final layer is not an instance of IOutputLayer. " & "Final layer is of type: " & OutputLayer.GetType())
			End If

			Dim wsm As WorkspaceMode = (If(training, layerWiseConfigurations_Conflict.getTrainingWorkspaceMode(), layerWiseConfigurations_Conflict.getInferenceWorkspaceMode()))
			Dim mgr As LayerWorkspaceMgr
			If wsm = WorkspaceMode.NONE Then
				mgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				mgr = LayerWorkspaceMgr.builder().with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).noWorkspaceFor(ArrayType.ACTIVATIONS).noWorkspaceFor(ArrayType.INPUT).build()
			End If
			mgr.setHelperWorkspacePointers(helperWorkspaces)

			Dim inputToOutputLayer As INDArray = outputOfLayerDetached(training, FwdPassType.STANDARD,layers_Conflict.Length-2, data.Features, data.FeaturesMaskArray, data.LabelsMaskArray, Nothing)

			If data.Features.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim ol As IOutputLayer = DirectCast(OutputLayer, IOutputLayer)
			If LayerWiseConfigurations.getInputPreProcess(layers_Conflict.Length - 1) IsNot Nothing Then
				inputToOutputLayer = LayerWiseConfigurations.getInputPreProcess(layers_Conflict.Length - 1).preProcess(inputToOutputLayer, CInt(data.Features.size(0)), mgr)
			End If
			ol.setInput(inputToOutputLayer, mgr) 'Feedforward doesn't include output layer for efficiency
			ol.Labels = data.Labels
			Dim score As Double
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
				score = ol.computeScore(calcRegularizationScore(True), training, mgr)
			End Using

			If hasMaskArray Then
				clearLayerMaskArrays()
			End If
			clearLayersStates()

			Return score
		End Function

		''' <summary>
		''' As per <seealso cref="scoreExamples(DataSet, Boolean)"/> - the outputs (example scores) for all DataSets in the iterator are concatenated
		''' </summary>
		Public Overridable Function scoreExamples(ByVal iter As DataSetIterator, ByVal addRegularizationTerms As Boolean) As INDArray
			Dim [out] As IList(Of INDArray) = New List(Of INDArray)()

			Do While iter.MoveNext()
				[out].Add(scoreExamples(iter.Current, addRegularizationTerms))
			Loop
			Return Nd4j.toFlattened("f"c, [out])
		End Function

		''' <summary>
		'''Calculate the score for each example in a DataSet individually. Unlike <seealso cref="score(DataSet)"/> and <seealso cref="score(DataSet, Boolean)"/>
		''' this method does not average/sum over examples. This method allows for examples to be scored individually (at test time only), which
		''' may be useful for example for autoencoder architectures and the like.<br>
		''' Each row of the output (assuming addRegularizationTerms == true) is equivalent to calling score(DataSet) with a single example. </summary>
		''' <param name="data"> The data to score </param>
		''' <param name="addRegularizationTerms"> If true: add l1/l2 regularization terms (if any) to the score. If false: don't add regularization terms </param>
		''' <returns> An INDArray (column vector) of size input.numRows(); the ith entry is the score (loss value) of the ith example </returns>
		Public Overridable Function scoreExamples(ByVal data As DataSet, ByVal addRegularizationTerms As Boolean) As INDArray
			Try
				Return scoreExamplesHelper(data, addRegularizationTerms)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Private Function scoreExamplesHelper(ByVal data As DataSet, ByVal addRegularizationTerms As Boolean) As INDArray
			Dim inputLast As INDArray = outputOfLayerDetached(False, FwdPassType.STANDARD,layers_Conflict.Length-2, data.Features, data.FeaturesMaskArray, data.LabelsMaskArray, Nothing)
			Labels = data.Labels
			setLayerMaskArrays(data.FeaturesMaskArray, data.LabelsMaskArray)

			'TODO we might want workspaces here?
			Dim mgr As LayerWorkspaceMgr = LayerWorkspaceMgr.noWorkspaces()

			Dim [out] As INDArray
			If TypeOf OutputLayer Is IOutputLayer Then
				Dim ol As IOutputLayer = DirectCast(OutputLayer, IOutputLayer)
				If layerWiseConfigurations_Conflict.getInputPreProcess(layers_Conflict.Length-1) IsNot Nothing Then

					If data.Features.size(0) > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					inputLast = layerWiseConfigurations_Conflict.getInputPreProcess(layers_Conflict.Length-1).preProcess(inputLast, CInt(data.Features.size(0)), mgr)
				End If
				ol.Labels = data.Labels
				ol.setInput(inputLast, mgr)
				Dim r As Double = (If(addRegularizationTerms, calcRegularizationScore(True), 0))
				[out] = ol.computeScoreForExamples(r, mgr)
			Else
				Throw New System.NotSupportedException("Cannot calculate score with respect to labels without an OutputLayer")
			End If

			clearLayersStates()
			clearLayerMaskArrays()
			Return [out]
		End Function


		Public Overridable Sub fit() Implements org.deeplearning4j.nn.api.Model.fit
			fit(input_Conflict, labels_Conflict)
		End Sub

		Public Overridable Sub update(ByVal gradient As INDArray, ByVal paramType As String) Implements org.deeplearning4j.nn.api.Model.update
			Throw New System.NotSupportedException("Not implemented")
		End Sub


		''' <summary>
		''' Score of the model (relative to the objective function) - previously calculated on the last minibatch
		''' </summary>
		''' <returns> the score of the model (relative to the objective function) </returns>
		Public Overridable Function score() As Double Implements org.deeplearning4j.nn.api.Model.score
			Return score_Conflict
		End Function

		''' <summary>
		''' Intended for developer/internal use
		''' </summary>
		Public Overridable WriteOnly Property Score As Double
			Set(ByVal score As Double)
				Me.score_Conflict = score
			End Set
		End Property

		Public Overridable Sub computeGradientAndScore(ByVal layerWorkspaceMgr As LayerWorkspaceMgr) Implements org.deeplearning4j.nn.api.Model.computeGradientAndScore
			computeGradientAndScore()
		End Sub

		Public Overridable Sub computeGradientAndScore()

			If Not (TypeOf OutputLayer Is IOutputLayer) Then
				Throw New DL4JException("Cannot calculate gradient and score with respect to labels: final layer is not an IOutputLayer. " & "Final layer class: " & OutputLayer.GetType() & ". To calculate gradients and fit a network " & "using backpropagation, the final layer must be an output layer")
			End If

			'Note: Workspace manager is only ose here for score calculation... other workspace managers are used in the
			' various FF/backprop methds
			Dim mgr As LayerWorkspaceMgr
			If layerWiseConfigurations_Conflict.getTrainingWorkspaceMode() = WorkspaceMode.NONE Then
				mgr = LayerWorkspaceMgr.noWorkspaces()
			Else
				mgr = LayerWorkspaceMgr.builder().with(ArrayType.INPUT, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.ACTIVATIONS, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG).with(ArrayType.FF_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.BP_WORKING_MEM, WS_LAYER_WORKING_MEM, WS_LAYER_WORKING_MEM_CONFIG).with(ArrayType.RNN_FF_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).with(ArrayType.RNN_BP_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM, WS_RNN_LOOP_WORKING_MEM_CONFIG).build()

				If layerWiseConfigurations_Conflict.getCacheMode() IsNot Nothing Then
					'For now: store cache mode activations in activations workspace
					mgr.setWorkspace(ArrayType.FF_CACHE, WS_ALL_LAYERS_ACT, WS_ALL_LAYERS_ACT_CONFIG)
				End If
			End If

			Dim tbptt As Boolean = layerWiseConfigurations_Conflict.getBackpropType() = BackpropType.TruncatedBPTT
			Dim fwdType As FwdPassType = (If(tbptt, FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE, FwdPassType.STANDARD))
			synchronizeIterEpochCounts()

			'Calculate activations (which are stored in each layer, and used in backprop)
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.ACTIVATIONS)
				'First: do a feed-forward through the network
				'Note that we don't actually need to do the full forward pass through the output layer right now; but we do
				' need the input to the output layer to be set (such that backprop can be done)
				Dim activations As IList(Of INDArray) = ffToLayerActivationsInWs(layers_Conflict.Length - 2, fwdType, tbptt, input_Conflict, mask_Conflict, Nothing)
				If trainingListeners.Count > 0 Then
					'TODO: We possibly do want output layer activations in some cases here...
					For Each tl As TrainingListener In trainingListeners
						tl.onForwardPass(Me, activations)
					Next tl
				End If
				Dim inputToOutputLayer As INDArray = activations(activations.Count - 1)
				If layerWiseConfigurations_Conflict.getInputPreProcess(layers_Conflict.Length - 1) IsNot Nothing Then
					inputToOutputLayer = layerWiseConfigurations_Conflict.getInputPreProcess(layers_Conflict.Length - 1).preProcess(inputToOutputLayer, InputMiniBatchSize, mgr)
					'Validate activations location
				End If
				OutputLayer.setInput(inputToOutputLayer, mgr)
				'Then: compute gradients
				Dim pair As Pair(Of Gradient, INDArray) = calcBackpropGradients(Nothing, True, False, False)
				Me.gradient_Conflict = (If(pair Is Nothing, Nothing, pair.First))

				'Calculate score
				Using wsFF As org.nd4j.linalg.api.memory.MemoryWorkspace = mgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.FF_WORKING_MEM)
					Dim r As Double = calcRegularizationScore(True)
					score_Conflict = DirectCast(OutputLayer, IOutputLayer).computeScore(r, True, mgr)
				End Using

				'Listeners
				If trainingListeners.Count > 0 Then
					Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
						For Each tl As TrainingListener In trainingListeners
							tl.onBackwardPass(Me)
						Next tl
					End Using
				End If
			End Using

			'Clear the post noise/dropconnect parameters on the output layer
			OutputLayer.clearNoiseWeightParams()
		End Sub

		''' <summary>
		''' Clear the inputs. Clears optimizer state.
		''' </summary>
		Public Overridable Sub clear() Implements org.deeplearning4j.nn.api.Model.clear
			For Each layer As Layer In layers_Conflict
				layer.clear()
			Next layer

			input_Conflict = Nothing
			labels_Conflict = Nothing
			solver = Nothing
		End Sub

		Public Overridable Sub applyConstraints(ByVal iteration As Integer, ByVal epoch As Integer) Implements org.deeplearning4j.nn.api.Model.applyConstraints
			For Each l As Layer In layers_Conflict
				l.applyConstraints(iteration, epoch)
			Next l
		End Sub


		''' <summary>
		''' Set the input array for the network
		''' </summary>
		''' <param name="input"> Input array to set </param>
		Public Overridable Property Input As INDArray
			Set(ByVal input As INDArray)
				Me.input_Conflict = input
				If Me.layers_Conflict Is Nothing Then
					init()
				End If
				If input IsNot Nothing Then
					If input.length() = 0 Then
						Throw New System.ArgumentException("Invalid input: length 0 (shape: " & java.util.Arrays.toString(input.shape()) & ")")
					End If
    
					If input.size(0) > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					InputMiniBatchSize = CInt(input.size(0))
				End If
			End Set
			Get
				Return input_Conflict
			End Get
		End Property

		Public Overridable Sub setInput(ByVal input As INDArray, ByVal mgr As LayerWorkspaceMgr) Implements Layer.setInput
			Throw New System.NotSupportedException("Not supported")
		End Sub

		''' <summary>
		''' Get the output layer - i.e., the last layer in the netwok
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property OutputLayer As Layer
			Get
				Dim ret As Layer = Layers(Layers.Length - 1)
				If TypeOf ret Is FrozenLayerWithBackprop Then
					ret = DirectCast(ret, FrozenLayerWithBackprop).InsideLayer
				End If
				Return ret
			End Get
		End Property


		''' <summary>
		''' See <seealso cref="setParams(INDArray)"/>
		''' </summary>
		Public Overridable WriteOnly Property Parameters As INDArray
			Set(ByVal params As INDArray)
				Me.Params = params
			End Set
		End Property

		''' <summary>
		''' Intended for internal/developer use
		''' </summary>
		Public Overridable ReadOnly Property DefaultConfiguration As NeuralNetConfiguration
			Get
				Return defaultConfiguration_Conflict
			End Get
		End Property

		Public Overridable Property Labels As INDArray
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels As INDArray)
				Me.labels_Conflict = labels
			End Set
		End Property




		''' <summary>
		''' Get the number of layers in the network
		''' </summary>
		''' <returns> the number of layers in the network </returns>
		Public Overridable Function getnLayers() As Integer
			Return layerWiseConfigurations_Conflict.getConfs().size()
		End Function

		''' <returns> The layers in the network </returns>
		Public Overridable Property Layers As Layer()
			Get
				SyncLock Me
					Return layers_Conflict
				End SyncLock
			End Get
			Set(ByVal layers() As Layer)
				Me.layers_Conflict = layers
			End Set
		End Property

		Public Overridable Function getLayer(ByVal i As Integer) As Layer
			Preconditions.checkArgument(i >= 0 AndAlso i < layers_Conflict.Length, "Invalid layer index: layer index must be 0" & " to %s (inclusive), got index %s", layers_Conflict.Length-1, i)
			Return layers_Conflict(i)
		End Function

		Public Overridable Function getLayer(ByVal name As String) As Layer
			Return layerMap.get(name)
		End Function

		Public Overridable ReadOnly Property LayerNames As IList(Of String)
			Get
				Return New List(Of String)(layerMap.keySet())
			End Get
		End Property


		Public Overridable Property Mask As INDArray
			Get
				Return mask_Conflict
			End Get
			Set(ByVal mask As INDArray)
				Me.mask_Conflict = mask
			End Set
		End Property


		Public Overridable Property MaskArray As INDArray Implements Layer.getMaskArray
			Get
				Return mask_Conflict
			End Get
			Set(ByVal maskArray As INDArray)
				Throw New System.NotSupportedException()
			End Set
		End Property

		Public Overridable ReadOnly Property PretrainLayer As Boolean Implements Layer.isPretrainLayer
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub clearNoiseWeightParams() Implements Layer.clearNoiseWeightParams
			For Each l As Layer In layers_Conflict
				l.clearNoiseWeightParams()
			Next l
		End Sub

		Public Overridable Sub allowInputModification(ByVal allow As Boolean) Implements Layer.allowInputModification
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState) Implements Layer.feedForwardMaskArray
			If maskArray Is Nothing Then
				For i As Integer = 0 To layers_Conflict.Length - 1
					layers_Conflict(i).feedForwardMaskArray(Nothing, Nothing, minibatchSize)
				Next i
			Else
				'Do a forward pass through each preprocessor and layer
				For i As Integer = 0 To layers_Conflict.Length - 1
					Dim preProcessor As InputPreProcessor = LayerWiseConfigurations.getInputPreProcess(i)

					If preProcessor IsNot Nothing Then
						Dim p As Pair(Of INDArray, MaskState) = preProcessor.feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
						If p IsNot Nothing Then
							maskArray = p.First
							currentMaskState = p.Second
						Else
							maskArray = Nothing
							currentMaskState = Nothing
						End If
					End If

					Dim p As Pair(Of INDArray, MaskState) = layers_Conflict(i).feedForwardMaskArray(maskArray, currentMaskState, minibatchSize)
					If p IsNot Nothing Then
						maskArray = p.First
						currentMaskState = p.Second
					Else
						maskArray = Nothing
						currentMaskState = Nothing
					End If
				Next i
			End If

			Return New Pair(Of INDArray, MaskState)(maskArray, currentMaskState)
		End Function

		Public Overridable ReadOnly Property Helper As LayerHelper Implements Layer.getHelper
			Get
				Throw New System.NotSupportedException("Not supported")
			End Get
		End Property

		'==========
		'Layer methods

		Public Overridable Function type() As Type Implements Layer.type
			Return Type.MULTILAYER
		End Function


		''' <summary>
		''' Equivalent to <seealso cref="output(INDArray)"/> using the input set via <seealso cref="setInput(INDArray)"/>
		''' </summary>
		Public Overridable Function activate(ByVal training As TrainingMode) As INDArray
			Return output(input_Conflict, training = TrainingMode.TRAIN)
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="output(INDArray, TrainingMode)"/>
		''' </summary>
		Public Overridable Function activate(ByVal input As INDArray, ByVal training As TrainingMode) As INDArray
			Return output(input, training = TrainingMode.TRAIN)
		End Function

		Public Overridable Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray) Implements Layer.backpropGradient
			If TypeOf OutputLayer Is IOutputLayer Then
				Throw New System.NotSupportedException("Cannot calculate gradients based on epsilon with OutputLayer")
			End If

			Return calcBackpropGradients(epsilon, False, False, True)
		End Function

		Public Overridable Property Index Implements Layer.setIndex As Integer
			Set(ByVal index As Integer)
				layerIndex = index
			End Set
			Get
				Return layerIndex
			End Get
		End Property


		Public Overridable Property IterationCount As Integer Implements Layer.getIterationCount
			Get
				Return LayerWiseConfigurations.getIterationCount()
			End Get
			Set(ByVal iterationCount As Integer)
				LayerWiseConfigurations.setIterationCount(iterationCount)
			End Set
		End Property

		Public Overridable Property EpochCount As Integer Implements Layer.getEpochCount
			Get
				Return LayerWiseConfigurations.EpochCount
			End Get
			Set(ByVal epochCount As Integer)
				LayerWiseConfigurations.EpochCount = epochCount
			End Set
		End Property



		Public Overridable Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double Implements Layer.calcRegularizationScore
			Dim scoreSum As Double = 0.0
			For i As Integer = 0 To layers_Conflict.Length - 1
				scoreSum += layers_Conflict(i).calcRegularizationScore(backpropParamsOnly)
			Next i
			Return scoreSum
		End Function

		Public Overridable Sub update(ByVal gradient As Gradient) Implements org.deeplearning4j.nn.api.Model.update
			If gradient.gradient().length() <> numParams(True) Then
				Throw New System.ArgumentException("Invalid input: expect gradients array of length " & numParams(True))
			End If
			For Each entry As KeyValuePair(Of String, INDArray) In gradient.gradientForVariable().SetOfKeyValuePairs()
				Dim key As String = entry.Key
				Dim val As INDArray = entry.Value
				Dim idx As Integer = key.IndexOf("_"c)
				If idx = -1 Then
					Throw New System.InvalidOperationException("Invalid param key: not have layer separator: """ & key & """")
				End If
				Dim layerId As Integer? = Integer.Parse(key.Substring(0, idx))
				Dim paramType As String = key.Substring(idx + 1)
				' Update MLN gradient
				Me.gradient_Conflict.gradientForVariable()(key) = val
				' Update layer params
				layers_Conflict(layerId).update(val, paramType)
			Next entry
			' Update layerwise gradient view
			BackpropGradientsViewArray = gradient.gradient()

		End Sub

		Public Overridable Function activate(ByVal training As Boolean, ByVal mgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal mgr As LayerWorkspaceMgr) As INDArray Implements Layer.activate
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Property InputMiniBatchSize Implements Layer.setInputMiniBatchSize As Integer
			Set(ByVal size As Integer)
				If layers_Conflict IsNot Nothing Then
					For Each l As Layer In layers_Conflict
						l.InputMiniBatchSize = size
					Next l
				End If
			End Set
			Get
				If Not conf().isMiniBatch() Then
					Return 1
				End If
    
				If input_Conflict.size(0) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				Return CInt(input_Conflict.size(0))
			End Get
		End Property



		''' 
		''' <summary>
		''' If this MultiLayerNetwork contains one or more RNN layers: conduct forward pass (prediction)
		''' but using previous stored state for any RNN layers. The activations for the final step are
		''' also stored in the RNN layers for use next time rnnTimeStep() is called.<br>
		''' This method can be used to generate output one or more steps at a time instead of always having to do
		''' forward pass from t=0. Example uses are for streaming data, and for generating samples from network output
		''' one step at a time (where samples are then fed back into the network as input)<br>
		''' If no previous state is present in RNN layers (i.e., initially or after calling rnnClearPreviousState()),
		''' the default initialization (usually 0) is used.<br>
		''' Supports mini-batch (i.e., multiple predictions/forward pass in parallel) as well as for single examples.<br> </summary>
		''' <param name="input"> Input to network. May be for one or multiple time steps. For single time step:
		'''  input has shape [miniBatchSize,inputSize] or [miniBatchSize,inputSize,1]. miniBatchSize=1 for single example.<br>
		'''  For multiple time steps: [miniBatchSize,inputSize,inputTimeSeriesLength] </param>
		''' <returns> Output activations. If output is RNN layer (such as RnnOutputLayer): if input has shape [miniBatchSize,inputSize]
		''' i.e., is 2d, output has shape [miniBatchSize,outputSize] (i.e., also 2d).<br>
		''' Otherwise output is 3d [miniBatchSize,outputSize,inputTimeSeriesLength] when using RnnOutputLayer. </returns>
		''' <seealso cref= #rnnTimeStep(INDArray, MemoryWorkspace) For outputting the activations in the specified workspace </seealso>
		Public Overridable Function rnnTimeStep(ByVal input As INDArray) As INDArray
			Return rnnTimeStep(input, Nothing)
		End Function

		''' <summary>
		''' See <seealso cref="rnnTimeStep(INDArray)"/> for details<br>
		''' If no memory workspace is provided, the output will be detached (not in any workspace).<br>
		''' If a memory workspace is provided, the output activation array (i.e., the INDArray returned by this method)
		''' will be placed in the specified workspace. This workspace must be opened by the user before calling this method -
		''' and the user is responsible for (a) closing this workspace, and (b) ensuring the output array is not used out
		''' of scope (i.e., not used after closing the workspace to which it belongs - as this is likely to cause either
		''' an exception when used, or a crash).
		''' </summary>
		''' <param name="input">           Input activations </param>
		''' <param name="outputWorkspace"> Output workspace. May be null </param>
		''' <returns> The output/activations from the network (either detached or in the specified workspace if provided) </returns>
		Public Overridable Function rnnTimeStep(ByVal input As INDArray, ByVal outputWorkspace As MemoryWorkspace) As INDArray
			Try
				Dim inputIs2d As Boolean = input.rank() = 2
				Dim [out] As INDArray = outputOfLayerDetached(False, FwdPassType.RNN_TIMESTEP, layers_Conflict.Length - 1, input, Nothing, Nothing, outputWorkspace)
				If inputIs2d AndAlso [out].rank() = 3 AndAlso layers_Conflict(layers_Conflict.Length - 1).type() = Type.RECURRENT Then
					'Return 2d output with shape [miniBatchSize,nOut]
					' instead of 3d output with shape [miniBatchSize,nOut,1]
					Return [out].tensorAlongDimension(0, 1, 0)
				End If
				Return [out]
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		''' <summary>
		'''Get the state of the RNN layer, as used in rnnTimeStep(). </summary>
		''' <param name="layer"> Number/index of the layer. </param>
		''' <returns> Hidden state, or null if layer is not an RNN layer </returns>
		Public Overridable Function rnnGetPreviousState(ByVal layer As Integer) As IDictionary(Of String, INDArray)
			If layer < 0 OrElse layer >= layers_Conflict.Length Then
				Throw New System.ArgumentException("Invalid layer number")
			End If
			Dim l As Layer = layers_Conflict(layer)
			If TypeOf l Is BaseWrapperLayer Then
				l = DirectCast(l, BaseWrapperLayer).getUnderlying()
			End If
			If Not (TypeOf l Is RecurrentLayer) Then
				Throw New System.ArgumentException("Layer is not an RNN layer")
			End If
			Return DirectCast(l, RecurrentLayer).rnnGetPreviousState()
		End Function

		''' <summary>
		'''Set the state of the RNN layer. </summary>
		''' <param name="layer"> The number/index of the layer. </param>
		''' <param name="state"> The state to set the specified layer to </param>
		Public Overridable Sub rnnSetPreviousState(ByVal layer As Integer, ByVal state As IDictionary(Of String, INDArray))
			If layer < 0 OrElse layer >= layers_Conflict.Length Then
				Throw New System.ArgumentException("Invalid layer number")
			End If
			Dim l As Layer = layers_Conflict(layer)
			If TypeOf l Is BaseWrapperLayer Then
				l = DirectCast(l, BaseWrapperLayer).getUnderlying()
			End If
			If Not (TypeOf l Is RecurrentLayer) Then
				Throw New System.ArgumentException("Layer is not an RNN layer")
			End If
			Dim r As RecurrentLayer = DirectCast(l, RecurrentLayer)
			r.rnnSetPreviousState(state)
		End Sub

		''' <summary>
		''' Clear the previous state of the RNN layers (if any).
		''' </summary>
		Public Overridable Sub rnnClearPreviousState()
			If layers_Conflict Is Nothing Then
				Return
			End If
			For i As Integer = 0 To layers_Conflict.Length - 1
				If TypeOf layers_Conflict(i) Is RecurrentLayer Then
					DirectCast(layers_Conflict(i), RecurrentLayer).rnnClearPreviousState()
				ElseIf TypeOf layers_Conflict(i) Is MultiLayerNetwork Then
					DirectCast(layers_Conflict(i), MultiLayerNetwork).rnnClearPreviousState()
				ElseIf TypeOf layers_Conflict(i) Is BaseWrapperLayer AndAlso TypeOf (DirectCast(layers_Conflict(i), BaseWrapperLayer)).getUnderlying() Is RecurrentLayer Then
					DirectCast(DirectCast(layers_Conflict(i), BaseWrapperLayer).getUnderlying(), RecurrentLayer).rnnClearPreviousState()
				End If
			Next i
		End Sub

		''' <summary>
		''' Similar to rnnTimeStep and feedForward() methods. Difference here is that this method:<br>
		''' (a) like rnnTimeStep does forward pass using stored state for RNN layers, and<br>
		''' (b) unlike rnnTimeStep does not modify the RNN layer state<br>
		''' Therefore multiple calls to this method with the same input should have the same output.<br>
		''' Typically used during training only. Use rnnTimeStep for prediction/forward pass at test time. </summary>
		''' <param name="input"> Input to network </param>
		''' <param name="training"> Whether training or not </param>
		''' <param name="storeLastForTBPTT"> set to true if used as part of truncated BPTT training </param>
		''' <returns> Activations for each layer (including input, as per feedforward() etc) </returns>
		Public Overridable Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean) As IList(Of INDArray)
			Return ffToLayerActivationsDetached(training, FwdPassType.RNN_ACTIVATE_WITH_STORED_STATE, storeLastForTBPTT, layers_Conflict.Length-1, input, mask_Conflict, Nothing, False)
		End Function

		''' <summary>
		''' Get the updater for this MultiLayerNetwork </summary>
		''' <returns> Updater for MultiLayerNetwork </returns>
		Public Overridable Property Updater As Updater
			Get
				Return getUpdater(True)
			End Get
			Set(ByVal updater As Updater)
				If solver Is Nothing Then
					solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
				End If
				solver.Optimizer.Updater = updater
			End Set
		End Property

		Public Overridable Function getUpdater(ByVal initializeIfReq As Boolean) As Updater
			If solver Is Nothing AndAlso initializeIfReq Then
				SyncLock Me
					If solver Is Nothing Then 'May have been created while waiting for lock
						solver = (New Solver.Builder()).configure(conf()).listeners(getListeners()).model(Me).build()
						solver.Optimizer.Updater = UpdaterCreator.getUpdater(Me)
					End If
				End SyncLock
			End If
			If solver IsNot Nothing Then
				Return solver.Optimizer.getUpdater(initializeIfReq)
			End If
			Return Nothing
		End Function


		''' <summary>
		'''Set the mask arrays for features and labels. Mask arrays are typically used in situations such as one-to-many
		''' and many-to-one learning with recurrent neural networks, as well as for supporting time series of varying lengths
		''' within the same minibatch.<br>
		''' For example, with RNN data sets with input of shape [miniBatchSize,nIn,timeSeriesLength] and outputs of shape
		''' [miniBatchSize,nOut,timeSeriesLength], the features and mask arrays will have shape [miniBatchSize,timeSeriesLength]
		''' and contain values 0 or 1 at each element (to specify whether a given input/example is present - or merely padding -
		''' at a given time step).<br>
		''' <b>NOTE</b>: This method is not usually used directly. Instead, methods such as <seealso cref="feedForward(INDArray, INDArray, INDArray)"/>
		''' and <seealso cref="output(INDArray, Boolean, INDArray, INDArray)"/> handle setting of masking internally. </summary>
		''' <param name="featuresMaskArray"> Mask array for features (input) </param>
		''' <param name="labelsMaskArray"> Mask array for labels (output) </param>
		''' <seealso cref= #clearLayerMaskArrays() </seealso>
		Public Overridable Sub setLayerMaskArrays(ByVal featuresMaskArray As INDArray, ByVal labelsMaskArray As INDArray)
			If featuresMaskArray IsNot Nothing Then

				If featuresMaskArray.size(0) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				'New approach: use feedForwardMaskArray method
				feedForwardMaskArray(featuresMaskArray, MaskState.Active, CInt(featuresMaskArray.size(0)))


	'            
	'            //feedforward layers below a RNN layer: need the input (features) mask array
	'            //Reason: even if the time series input is zero padded, the output from the dense layers are
	'            // non-zero (i.e., activationFunction(0*weights + bias) != 0 in general)
	'            //This assumes that the time series input is masked - i.e., values are 0 at the padded time steps,
	'            // so we don't need to do anything for the recurrent layer
	'            
	'            //Now, if mask array is 2d -> need to reshape to 1d (column vector) in the exact same order
	'            // as is done for 3d -> 2d time series reshaping
	'            INDArray reshapedFeaturesMask = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(featuresMaskArray);
	'            
	'            for( int i=0; i<layers.length-1; i++ ){
	'                Type t = layers[i].type();
	'                if( t == Type.CONVOLUTIONAL || t == Type.FEED_FORWARD ){
	'                    layers[i].setMaskArray(reshapedFeaturesMask);
	'                } else if( t == Type.RECURRENT ) break;
	'            
	'            }
	'            
			End If
			If labelsMaskArray IsNot Nothing Then
				If Not (TypeOf OutputLayer Is IOutputLayer) Then
					Return
				End If
				layers_Conflict(layers_Conflict.Length - 1).MaskArray = labelsMaskArray
			End If
		End Sub

		''' <summary>
		''' Remove the mask arrays from all layers.<br>
		''' See <seealso cref="setLayerMaskArrays(INDArray, INDArray)"/> for details on mask arrays.
		''' </summary>
		Public Overridable Sub clearLayerMaskArrays()
			For Each layer As Layer In layers_Conflict
				layer.MaskArray = Nothing
			Next layer
		End Sub

		''' <summary>
		''' Evaluate the network (classification performance)
		''' </summary>
		''' <param name="iterator"> Iterator to evaluate on </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.evaluation.classification.Evaluation> T evaluate(@NonNull DataSetIterator iterator)
		Public Overridable Function evaluate(Of T As Evaluation)(ByVal iterator As DataSetIterator) As T
			Return CType(evaluate(iterator, Nothing), T)
		End Function

		''' <summary>
		''' Evaluate the network (classification performance).
		''' Can only be used with MultiDataSetIterator instances with a single input/output array
		''' </summary>
		''' <param name="iterator"> Iterator to evaluate on </param>
		''' <returns> Evaluation object; results of evaluation on all examples in the data set </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.evaluation.classification.Evaluation evaluate(@NonNull MultiDataSetIterator iterator)
		Public Overridable Function evaluate(ByVal iterator As MultiDataSetIterator) As Evaluation
			Return evaluate(New MultiDataSetWrapperIterator(iterator))
		End Function

		''' <summary>
		''' Evaluate the network for regression performance </summary>
		''' <param name="iterator"> Data to evaluate on </param>
		''' <returns> Regression evaluation </returns>
		Public Overridable Function evaluateRegression(Of T As RegressionEvaluation)(ByVal iterator As DataSetIterator) As T
			Return CType(doEvaluation(iterator, New RegressionEvaluation(iterator.totalOutcomes()))(0), T)
		End Function

		''' <summary>
		''' Evaluate the network for regression performance
		''' Can only be used with MultiDataSetIterator instances with a single input/output array </summary>
		''' <param name="iterator"> Data to evaluate on </param>
		Public Overridable Function evaluateRegression(ByVal iterator As MultiDataSetIterator) As RegressionEvaluation
			Return evaluateRegression(New MultiDataSetWrapperIterator(iterator))
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
		''' <param name="rocThresholdSteps"> Number of threshold steps to use with <seealso cref="ROC"/> - see that class for details. </param>
		''' <returns> ROC evaluation on the given dataset </returns>
		Public Overridable Function evaluateROC(Of T As ROC)(ByVal iterator As DataSetIterator, ByVal rocThresholdSteps As Integer) As T
			Dim outputLayer As Layer = Me.OutputLayer
			If LayerWiseConfigurations.isValidateOutputLayerConfig() Then
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
			Dim outputLayer As Layer = Me.OutputLayer
			If LayerWiseConfigurations.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(ROCMultiClass))
			End If
			Return CType(doEvaluation(iterator, New org.deeplearning4j.eval.ROCMultiClass(rocThresholdSteps))(0), T)
		End Function

		''' <summary>
		''' Perform evaluation using an arbitrary IEvaluation instance.
		''' </summary>
		''' <param name="iterator">   data to evaluate on </param>
		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal iterator As DataSetIterator, ParamArray ByVal evaluations() As T) As T() Implements NeuralNetwork.doEvaluation
			Try
				Return doEvaluationHelper(iterator, evaluations)
			Catch e As System.OutOfMemoryException
				CrashReportingUtil.writeMemoryCrashDump(Me, e)
				Throw e
			End Try
		End Function

		Public Overridable Function doEvaluationHelper(Of T As IEvaluation)(ByVal iterator As DataSetIterator, ParamArray ByVal evaluations() As T) As T()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() AndAlso iterator.resetSupported() Then
				iterator.reset()
			End If

			Dim iter As DataSetIterator = If(iterator.asyncSupported(), New AsyncDataSetIterator(iterator, 2, True), iterator)

			Dim cMode As WorkspaceMode = layerWiseConfigurations_Conflict.getTrainingWorkspaceMode()
			layerWiseConfigurations_Conflict.setTrainingWorkspaceMode(layerWiseConfigurations_Conflict.getInferenceWorkspaceMode())

			'First: let's determine if we should do 'split feed forward' for long time series
			'The idea: RNN 20k time steps. Train using TBPTT length 100 -> 200 segments of length 100. If we naively
			' just use .output(INDArray) here, then our memory requirements are 200x larger than if we did the same
			' evaluation in segments...
			'Only do this if TBPTT is enabled - if not, it means we can train without TBPTT and hence should be able
			' to test without splitting also
			Dim useRnnSegments As Boolean = (layerWiseConfigurations_Conflict.getBackpropType() = BackpropType.TruncatedBPTT)

			Dim outputWs As MemoryWorkspace
			If LayerWiseConfigurations.getInferenceWorkspaceMode() = WorkspaceMode.ENABLED Then
				outputWs = Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(WS_ALL_LAYERS_ACT_CONFIG, WS_OUTPUT_MEM)
			Else
				outputWs = New DummyWorkspace()
			End If

			Do While iter.MoveNext()
				Dim [next] As DataSet = iter.Current

				If [next].Features Is Nothing OrElse [next].Labels Is Nothing Then
					Continue Do
				End If


				Dim features As INDArray = [next].Features
				Dim labels As INDArray = [next].Labels
				Dim fMask As INDArray = [next].FeaturesMaskArray
				Dim lMask As INDArray = [next].LabelsMaskArray
				Dim meta As IList(Of Serializable) = [next].getExampleMetaData()


				If Not useRnnSegments Then
					'Standard/non-RNN case:
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = outputWs.notifyScopeEntered()
						Dim [out] As INDArray = outputOfLayerDetached(False, FwdPassType.STANDARD, layers_Conflict.Length - 1, features, fMask, lMask, ws)

						Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
							For Each evaluation As T In evaluations
								evaluation.eval(labels, [out], lMask, meta)
							Next evaluation
						End Using
					End Using
				Else
					rnnClearPreviousState()


					'Get subset of features and labels:
					Dim fwdLen As val = layerWiseConfigurations_Conflict.getTbpttFwdLength()
					Dim tsLength As val = features.size(2)
					Dim nSubsets As Long = tsLength / fwdLen
					If tsLength Mod fwdLen <> 0 Then
						nSubsets += 1 'Example: 100 fwdLen with timeSeriesLength=120 -> want 2 subsets (1 of size 100, 1 of size 20)
					End If
					For i As Integer = 0 To nSubsets - 1
						Dim startTimeIdx As val = i * fwdLen
						Dim endTimeIdx As val = Math.Min(startTimeIdx + fwdLen, tsLength)

						If endTimeIdx > Integer.MaxValue Then
							Throw New ND4JArraySizeException()
						End If
						Dim subsets() As INDArray = getSubsetsForTbptt(startTimeIdx, CInt(endTimeIdx), features, labels, fMask, lMask)

						setLayerMaskArrays(subsets(2), subsets(3))

						Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = outputWs.notifyScopeEntered()
							Dim outSub As INDArray = rnnTimeStep(subsets(0), ws)
							Using wsO As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
								For Each evaluation As T In evaluations
									evaluation.eval(subsets(1), outSub, subsets(3))
								Next evaluation
							End Using
						End Using
					Next i
				End If

				'Clear inputs, masks etc. Important to avoid leaking invalidated/out of scope arrays between iterations
				clearLayersStates()
			Loop

			If iterator.asyncSupported() Then
				DirectCast(iter, AsyncDataSetIterator).shutdown()
			End If

			layerWiseConfigurations_Conflict.setTrainingWorkspaceMode(cMode)

			Return evaluations
		End Function

		''' <summary>
		''' Evaluate the network on the provided data set. Used for evaluating the performance of classifiers
		''' </summary>
		''' <param name="iterator"> Data to undertake evaluation on </param>
		''' <returns> Evaluation object, summarizing the results of the evaluation on the provided DataSetIterator </returns>
		Public Overridable Function evaluate(ByVal iterator As DataSetIterator, ByVal labelsList As IList(Of String)) As Evaluation
			Return evaluate(iterator, labelsList, 1)
		End Function

		Public Overridable Function updaterState() As INDArray Implements NeuralNetwork.updaterState
			Return If(Updater IsNot Nothing, Updater.StateViewArray, Nothing)
		End Function

		Public Overridable Sub fit(ByVal dataSet As MultiDataSet) Implements NeuralNetwork.fit
			If dataSet.Features.Length = 1 AndAlso dataSet.Labels.Length = 1 Then
				Dim features As INDArray = dataSet.getFeatures(0)
				Dim labels As INDArray = dataSet.getLabels(0)
				Dim fMask As INDArray = Nothing
				Dim lMask As INDArray = Nothing

				If dataSet.FeaturesMaskArrays IsNot Nothing Then
					fMask = dataSet.FeaturesMaskArrays(0)
				End If

				If dataSet.FeaturesMaskArrays IsNot Nothing Then
					lMask = dataSet.LabelsMaskArrays(0)
				End If

				Dim ds As New DataSet(features, labels, fMask, lMask)
				fit(ds)
			Else
				Throw New DL4JInvalidInputException("MultiLayerNetwork can't handle MultiDataSet with more than 1 features or labels array." & "Please consider use of ComputationGraph")
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
			Preconditions.checkArgument(numEpochs = 1 OrElse iterator.resetSupported(), "Cannot perform multiple epochs training using" & "iterator has does not support resetting (iterator.resetSupported() returned false)")

			For i As Integer = 0 To numEpochs - 1
				fit(iterator)
			Next i
		End Sub

		''' <summary>
		''' Perform minibatch training on all minibatches in the MultiDataSetIterator.<br>
		''' Note: The MultiDataSets in the MultiDataSetIterator must have exactly 1 input and output array (as
		''' MultiLayerNetwork only supports 1 input and 1 output)
		''' </summary>
		''' <param name="iterator">  Training data (DataSetIterator). Iterator must support resetting </param>
		Public Overridable Sub fit(ByVal iterator As MultiDataSetIterator) Implements NeuralNetwork.fit
			fit(New MultiDataSetWrapperIterator(iterator))
		End Sub

		Public Overridable Function doEvaluation(Of T As IEvaluation)(ByVal iterator As MultiDataSetIterator, ByVal evaluations() As T) As T() Implements NeuralNetwork.doEvaluation
			Return doEvaluation(New MultiDataSetWrapperIterator(iterator), evaluations)
		End Function

		''' <summary>
		''' Evaluate the network (for classification) on the provided data set, with top N accuracy in addition to standard accuracy.
		''' For 'standard' accuracy evaluation only, use topN = 1
		''' </summary>
		''' <param name="iterator">   Iterator (data) to evaluate on </param>
		''' <param name="labelsList"> List of labels. May be null. </param>
		''' <param name="topN">       N value for top N accuracy evaluation </param>
		''' <returns> Evaluation object, summarizing the results of the evaluation on the provided DataSetIterator </returns>
		Public Overridable Function evaluate(ByVal iterator As DataSetIterator, ByVal labelsList As IList(Of String), ByVal topN As Integer) As Evaluation
			If layers_Conflict Is Nothing OrElse Not (TypeOf OutputLayer Is IOutputLayer) Then
				Throw New System.InvalidOperationException("Cannot evaluate network with no output layer")
			End If
			If labelsList Is Nothing Then
				Try
					labelsList = iterator.getLabels()
				Catch t As Exception
				End Try 'Ignore, maybe UnsupportedOperationException etc
			End If

			Dim outputLayer As Layer = Me.OutputLayer
			If LayerWiseConfigurations.isValidateOutputLayerConfig() Then
				OutputLayerUtil.validateOutputLayerForClassifierEvaluation(outputLayer.conf().getLayer(), GetType(Evaluation))
			End If

			Dim e As Evaluation = New org.deeplearning4j.eval.Evaluation(labelsList, topN)
			doEvaluation(iterator, e)

			Return e
		End Function

		Protected Friend Overridable Sub update(ByVal task As Task)
			If Not initDone Then
				initDone = True
				Dim heartbeat As Heartbeat = Heartbeat.Instance
				task = ModelSerializer.taskByModel(Me)
				Dim env As Environment = EnvironmentUtils.buildEnvironment()
				heartbeat.reportEvent([Event].STANDALONE, env, task)
			End If
		End Sub

		''' <summary>
		''' String detailing the architecture of the multilayernetwork.
		''' Columns are LayerIndex with layer type, nIn, nOut, Total number of parameters and the Shapes of the parameters
		''' Will also give information about frozen layers, if any. </summary>
		''' <returns> Summary as a string </returns>
		''' <seealso cref= #memoryInfo(int, InputType) </seealso>
		Public Overridable Function summary() As String
			Return summary(Nothing)
		End Function

		''' <summary>
		''' String detailing the architecture of the multilayernetwork.
		''' Will also display activation size when given an input type.
		''' Columns are LayerIndex with layer type, nIn, nOut, Total number of parameters, Shapes of the parameters, Input activation shape, Output activation shape
		''' Will also give information about frozen layers, if any. </summary>
		''' <returns> Summary as a string </returns>
		''' <seealso cref= #memoryInfo(int, InputType) </seealso>
		Public Overridable Function summary(ByVal inputType As InputType) As String
			Dim ret As New StringBuilder()
			ret.Append(vbLf)

			Dim lines As IList(Of String()) = New List(Of String())()
			If inputType Is Nothing Then
				lines.Add(New String(){"LayerName (LayerType)", "nIn,nOut", "TotalParams", "ParamsShape"})
			Else
				lines.Add(New String(){"LayerName (LayerType)", "nIn,nOut", "TotalParams", "ParamsShape", "InputShape", "OutputShape"})
			End If
			Dim maxLength(If(inputType Is Nothing, 4, 6) - 1) As Integer
			Dim header() As String = lines(0)
			For i As Integer = 0 To header.Length - 1
				maxLength(i) = header(i).Length
			Next i

			Dim frozenParams As Integer = 0
			For Each currentLayer As Layer In Layers
				Dim name As String = currentLayer.conf().getLayer().getLayerName()
				If name Is Nothing Then
					name = currentLayer.Index.ToString()
				End If
				Dim paramShape As String = "-"
				Dim [in] As String = "-"
				Dim [out] As String = "-"
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim classNameArr() As String = currentLayer.GetType().FullName.Split("\.", True)
				Dim className As String = classNameArr(classNameArr.Length - 1)
'JAVA TO VB CONVERTER TODO TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Dim paramCount As String = String.Format("%,d", currentLayer.numParams())
				Dim inShape As String = ""
				Dim outShape As String = ""
				Dim preProcessor As InputPreProcessor
				Dim outType As InputType
				If inputType IsNot Nothing Then
					preProcessor = LayerWiseConfigurations.getInputPreProcess(currentLayer.Index)
					inShape = inputType.ToString()
					If preProcessor IsNot Nothing Then
						inputType = preProcessor.getOutputType(inputType)
						inShape &= "--> " & inputType.ToString()
					End If
					outType = currentLayer.conf().getLayer().getOutputType(currentLayer.Index, inputType)
					outShape = outType.ToString()
					inputType = outType
				End If
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
					Dim paraNames As ISet(Of String) = currentLayer.paramTable().Keys
					For Each aP As String In paraNames
						Dim paramS As String = ArrayUtils.toString(currentLayer.paramTable()(aP).shape())
						paramShape &= aP & ":" & paramS & ", "
					Next aP
					paramShape = paramShape.Substring(0, paramShape.LastIndexOf(",", StringComparison.Ordinal)).ToString()
				End If
				If TypeOf currentLayer Is FrozenLayer Then
					frozenParams += currentLayer.numParams()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					classNameArr = DirectCast(currentLayer, FrozenLayer).InsideLayer.GetType().FullName.Split("\.", True)
					className = "Frozen " & classNameArr(classNameArr.Length - 1)
				End If

				Dim line() As String
				If inputType Is Nothing Then
					line = New String(){name & " (" & className & ")", [in] & "," & [out], paramCount, paramShape}
				Else
					line = New String(){name & " (" & className & ")", [in] & "," & [out], paramCount, paramShape, inShape, outShape}
				End If
				For i As Integer = 0 To line.Length - 1
					maxLength(i) = Math.Max(maxLength(i),If(line(i) Is Nothing, 0, line(i).Length))
				Next i
				lines.Add(line)
			Next currentLayer

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

			ret.Append(StringUtils.repeat("-", totalLength))
			ret.Append(String.format(vbLf & "%30s %,d", "Total Parameters: ", params().length()))
			ret.Append(String.format(vbLf & "%30s %,d", "Trainable Parameters: ", params().length() - frozenParams))
			ret.Append(String.format(vbLf & "%30s %,d", "Frozen Parameters: ", frozenParams))
			ret.Append(vbLf)
			ret.Append(StringUtils.repeat("=", totalLength))
			ret.Append(vbLf)
			Return ret.ToString()
		End Function

		''' <summary>
		''' Generate information regarding memory use for the network, for the given input type and minibatch size.
		''' Note that when using workspaces or CuDNN, the network should be trained for some iterations so that the memory
		''' workspaces have time to initialize. Without this, the memory requirements during training may be underestimated.
		''' 
		''' Note also that this is the same information that is generated during an OOM crash when training or performing
		''' inference.
		''' </summary>
		''' <param name="minibatch">    Minibatch size to estimate memory for </param>
		''' <param name="inputType">    Input type to the network </param>
		''' <returns> A String with information about network memory use information </returns>
		Public Overridable Function memoryInfo(ByVal minibatch As Integer, ByVal inputType As InputType) As String
			Return CrashReportingUtil.generateMemoryStatus(Me, minibatch, inputType)
		End Function

		''' <summary>
		''' This method just makes sure there's no state preserved within layers
		''' </summary>
		Public Overridable Sub clearLayersStates()
			For Each layer As Layer In layers_Conflict
				layer.clear()
				layer.clearNoiseWeightParams()
			Next layer
		End Sub

		''' <summary>
		''' Increment the epoch count (in the underlying <seealso cref="MultiLayerConfiguration"/> by 1).
		''' Note that this is done <i>automatically</i> when using iterator-based fitting methods, such as
		''' <seealso cref="fit(DataSetIterator)"/>. However, when using non-iterator fit methods (DataSet, INDArray/INDArray etc),
		''' the network has no way to know when one epoch ends and another starts. In such situations, this method
		''' can be used to increment the epoch counter.<br>
		''' Note that the epoch counter is used for situations such as some learning rate schedules, and the like.
		''' 
		''' The current epoch count can be obtained using {@code MultiLayerConfiguration.getLayerwiseConfiguration().getEpochCount()}
		''' </summary>
		Public Overridable Sub incrementEpochCount()
			layerWiseConfigurations_Conflict.EpochCount = layerWiseConfigurations_Conflict.EpochCount + 1
			synchronizeIterEpochCounts()
		End Sub


		Protected Friend Overridable Sub synchronizeIterEpochCounts()
			'TODO: this is necessary for some schedules - but the redundant values are a little ugly...
			Dim currIter As Integer = IterationCount
			Dim currEpoch As Integer = EpochCount
			For Each l As Layer In layers_Conflict
				l.IterationCount = currIter
				l.EpochCount = currEpoch
			Next l
		End Sub

		''' <summary>
		''' Save the MultiLayerNetwork to a file. Restore using <seealso cref="load(File, Boolean)"/>.
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
		''' Save the MultiLayerNetwork to a file. Restore using <seealso cref="load(File, Boolean)"/>.
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
		''' Restore a MultiLayerNetwork to a file, saved using <seealso cref="save(File)"/> or <seealso cref="ModelSerializer"/> </summary>
		''' <param name="f"> File to load the network from </param>
		''' <param name="loadUpdater"> If true: load the updater if it is available (i.e., the state array for momentum/Adam/rmsprop
		'''                   etc) - use <i>false</i> if no further training is required, or <i>true</i> if further training
		'''                    will be undertaken </param>
		''' <seealso cref= ModelSerializer ModelSerializer for more details (and saving/loading via streams) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static MultiLayerNetwork load(File f, boolean loadUpdater) throws IOException
		Public Shared Function load(ByVal f As File, ByVal loadUpdater As Boolean) As MultiLayerNetwork
			Return ModelSerializer.restoreMultiLayerNetwork(f, loadUpdater)
		End Function

		''' <summary>
		''' Convert this MultiLayerNetwork to a ComputationGraph
		''' </summary>
		''' <returns> ComputationGraph equivalent to this network (including parameters and updater state) </returns>
		Public Overridable Function toComputationGraph() As ComputationGraph
			Return NetworkUtils.toComputationGraph(Me)
		End Function

		''' <summary>
		''' Return a copy of the network with the parameters and activations set to use the specified (floating point) data type.
		''' If the existing datatype is the same as the requested dataype, the original network will be returned unchanged.
		''' Only floating point datatypes (DOUBLE, FLOAT, HALF) may be used.
		''' </summary>
		''' <param name="dataType"> Datatype to convert the network to </param>
		''' <returns> The network, set to use the specified datatype for the parameters and activations </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MultiLayerNetwork convertDataType(@NonNull DataType dataType)
		Public Overridable Function convertDataType(ByVal dataType As DataType) As MultiLayerNetwork
			Preconditions.checkState(dataType.isFPType(), "Invalid DataType: %s. Can only convert network to a floating point type", dataType)
			If dataType = params().dataType() Then
				Return Me
			End If

			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Dim newParams As INDArray = params().castTo(dataType)
				Dim jsonConfig As String = LayerWiseConfigurations.toJson()
				Dim newConf As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(jsonConfig)
				newConf.setDataType(dataType)
				Dim newNet As New MultiLayerNetwork(newConf)
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
		''' <seealso cref= #setLearningRate(int, double) </seealso>
		Public Overridable WriteOnly Property LearningRate As Double
			Set(ByVal newLr As Double)
				NetworkUtils.setLearningRate(Me, newLr)
			End Set
		End Property

		''' <summary>
		''' Set the learning rate schedule for all layers in the network to the specified schedule.
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="MultiLayerConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="MultiLayerConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="newLr"> New learning rate schedule for all layers </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(int, double) </seealso>
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
		''' <param name="layerNumber"> Number of the layer to set the LR for </param>
		''' <param name="newLr"> New learning rate for a single layer </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(int, double) </seealso>
		Public Overridable Sub setLearningRate(ByVal layerNumber As Integer, ByVal newLr As Double)
			NetworkUtils.setLearningRate(Me, layerNumber, newLr)
		End Sub

		''' <summary>
		''' Set the learning rate schedule for a single layer in the network to the specified value.<br>
		''' Note also that <seealso cref="setLearningRate(ISchedule)"/> should also be used in preference, when all layers need
		''' to be set to a new LR schedule.<br>
		''' This schedule will replace any/all existing schedules, and also any fixed learning rate values.<br>
		''' Note also that the iteration/epoch counts will <i>not</i> be reset. Use <seealso cref="MultiLayerConfiguration.setIterationCount(Integer)"/>
		''' and <seealso cref="MultiLayerConfiguration.setEpochCount(Integer)"/> if this is required
		''' </summary>
		''' <param name="layerNumber"> Number of the layer to set the LR schedule for </param>
		''' <param name="newLr"> New learning rate for a single layer </param>
		''' <seealso cref= #setLearningRate(ISchedule) </seealso>
		''' <seealso cref= #setLearningRate(int, double) </seealso>
		Public Overridable Sub setLearningRate(ByVal layerNumber As Integer, ByVal newLr As ISchedule)
			NetworkUtils.setLearningRate(Me, layerNumber, newLr)
		End Sub

		''' <summary>
		''' Get the current learning rate, for the specified layer, from the network.
		''' Note: If the layer has no learning rate (no parameters, or an updater without a learning rate) then null is returned </summary>
		''' <param name="layerNumber">   Layer number to get the learning rate for </param>
		''' <returns> Learning rate for the specified layer, or null </returns>
		Public Overridable Function getLearningRate(ByVal layerNumber As Integer) As Double?
			Return NetworkUtils.getLearningRate(Me, layerNumber)
		End Function

		''' <summary>
		''' Return the layer size (number of units) for the specified layer.<br>
		''' Note that the meaning of the "layer size" can depend on the type of layer. For example:<br>
		''' - DenseLayer, OutputLayer, recurrent layers: number of units (nOut configuration option)<br>
		''' - ConvolutionLayer: the channels (number of channels)<br>
		''' - Subsampling layers, global pooling layers, etc: size of 0 is always returned<br>
		''' </summary>
		''' <param name="layer"> Index of the layer to get the size of. Must be in range 0 to nLayers-1 inclusive </param>
		''' <returns> Size of the layer </returns>
		Public Overridable Function layerSize(ByVal layer As Integer) As Integer
			If layer < 0 OrElse layer > layers_Conflict.Length Then
				Throw New System.ArgumentException("Invalid layer index: " & layer & ". Layer index must be between 0 and " & (layers_Conflict.Length - 1) & " inclusive")
			End If
			Dim conf As org.deeplearning4j.nn.conf.layers.Layer = layers_Conflict(layer).conf().getLayer()
			If conf Is Nothing OrElse Not (TypeOf conf Is FeedForwardLayer) Then
				Return 0
			End If
			Dim ffl As FeedForwardLayer = DirectCast(conf, FeedForwardLayer)

			If ffl.getNOut() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Return CInt(Math.Truncate(ffl.getNOut()))
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
		Public Overridable Function layerInputSize(ByVal layer As Integer) As Integer
			If layer < 0 OrElse layer > layers_Conflict.Length Then
				Throw New System.ArgumentException("Invalid layer index: " & layer & ". Layer index must be between 0 and " & (layers_Conflict.Length - 1) & " inclusive")
			End If
			Dim conf As org.deeplearning4j.nn.conf.layers.Layer = layers_Conflict(layer).conf().getLayer()
			If conf Is Nothing OrElse Not (TypeOf conf Is FeedForwardLayer) Then
				Return 0
			End If
			Dim ffl As FeedForwardLayer = DirectCast(conf, FeedForwardLayer)

			If ffl.getNIn() > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Return CInt(Math.Truncate(ffl.getNIn()))
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
			If TypeOf obj Is MultiLayerNetwork Then
				Dim network As MultiLayerNetwork = DirectCast(obj, MultiLayerNetwork)
				Dim paramsEquals As Boolean = network.params().Equals(params())
				Dim confEquals As Boolean = LayerWiseConfigurations.Equals(network.LayerWiseConfigurations)
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
			Dim mln As val = ModelSerializer.restoreMultiLayerNetwork(ois, True)

			Me.defaultConfiguration_Conflict = mln.defaultConfiguration.clone()
			Me.layerWiseConfigurations_Conflict = mln.layerWiseConfigurations.clone()
			Me.init()
			Me.flattenedParams.assign(mln.flattenedParams)

			Dim numWorkingMem As Integer = 2 * (layerWiseConfigurations_Conflict.getConfs().size() + layerWiseConfigurations_Conflict.getInputPreProcessors().size())
			WS_LAYER_WORKING_MEM_CONFIG = getLayerWorkingMemWSConfig(numWorkingMem)
			WS_LAYER_ACT_X_CONFIG = getLayerActivationWSConfig(layerWiseConfigurations_Conflict.getConfs().size())

			If mln.getUpdater() IsNot Nothing AndAlso mln.getUpdater(False).getStateViewArray() IsNot Nothing Then
				Me.getUpdater(True).StateViewArray.assign(mln.getUpdater(False).getStateViewArray())
			End If
		End Sub

		''' <summary>
		''' Close the network and deallocate all native memory, including: parameters, gradients, updater memory and workspaces
		''' Note that the network should not be used again for any purpose after it has been closed
		''' </summary>
		Public Overridable Sub close() Implements org.deeplearning4j.nn.api.Model.close
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