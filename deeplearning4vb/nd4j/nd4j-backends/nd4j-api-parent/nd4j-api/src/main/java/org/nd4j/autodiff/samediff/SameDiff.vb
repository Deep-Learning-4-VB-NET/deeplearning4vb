Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports ExecutorConfiguration = org.nd4j.autodiff.execution.conf.ExecutorConfiguration
Imports OutputMode = org.nd4j.autodiff.execution.conf.OutputMode
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports org.nd4j.autodiff.listeners
Imports HistoryListener = org.nd4j.autodiff.listeners.impl.HistoryListener
Imports History = org.nd4j.autodiff.listeners.records.History
Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports OutAndGrad = org.nd4j.autodiff.samediff.api.OutAndGrad
Imports SingleThreadArrayHolder = org.nd4j.autodiff.samediff.array.SingleThreadArrayHolder
Imports ThreadSafeArrayHolder = org.nd4j.autodiff.samediff.array.ThreadSafeArrayHolder
Imports BatchOutputConfig = org.nd4j.autodiff.samediff.config.BatchOutputConfig
Imports EvaluationConfig = org.nd4j.autodiff.samediff.config.EvaluationConfig
Imports FitConfig = org.nd4j.autodiff.samediff.config.FitConfig
Imports OutputConfig = org.nd4j.autodiff.samediff.config.OutputConfig
Imports org.nd4j.autodiff.samediff.internal
Imports org.nd4j.autodiff.samediff.ops
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports ROC = org.nd4j.evaluation.classification.ROC
Imports org.nd4j.graph
Imports VariableUtils = org.nd4j.imports.VariableUtils
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseOp = org.nd4j.linalg.api.ops.BaseOp
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports org.nd4j.linalg.api.ops.impl.controlflow.compat
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports TensorArray = org.nd4j.linalg.api.ops.impl.shape.tensorops.TensorArray
Imports Assert = org.nd4j.linalg.api.ops.impl.transforms.Assert
Imports GradientBackwardsMarker = org.nd4j.linalg.api.ops.impl.transforms.gradient.GradientBackwardsMarker
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports AsyncMultiDataSetIterator = org.nd4j.linalg.dataset.AsyncMultiDataSetIterator
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports SingletonMultiDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonMultiDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports ND4JIllegalArgumentException = org.nd4j.linalg.exception.ND4JIllegalArgumentException
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ND4UnresolvedOutputVariables = org.nd4j.linalg.exception.ND4UnresolvedOutputVariables
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.linalg.learning
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports ND4JFileUtils = org.nd4j.common.util.ND4JFileUtils
Imports HashBasedTable = org.nd4j.shade.guava.collect.HashBasedTable
Imports Sets = org.nd4j.shade.guava.collect.Sets
Imports Table = org.nd4j.shade.guava.collect.Table
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports WeightInitScheme = org.nd4j.weightinit.WeightInitScheme
Imports NDArraySupplierInitScheme = org.nd4j.weightinit.impl.NDArraySupplierInitScheme
Imports ZeroInitScheme = org.nd4j.weightinit.impl.ZeroInitScheme
Imports GraphDef = org.tensorflow.framework.GraphDef
import static org.nd4j.autodiff.util.SameDiffUtils.stackOutputs
import static org.nd4j.imports.VariableUtils.stripVarSuffix

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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SameDiff extends SDBaseOps
	Public Class SameDiff
		Inherits SDBaseOps

		Protected Friend Const GRAD_FN_KEY As String = "grad"

		'Fields for graph structure and execution
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<String, Variable> variables = new LinkedHashMap<>();
'JAVA TO VB CONVERTER NOTE: The field variables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly variables_Conflict As IDictionary(Of String, Variable) = New LinkedHashMap(Of String, Variable)() 'Use linked hash map to guarantee iteration order based on order they were added. Used in inputs() and flatbuffers serde
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<String, SameDiffOp> ops = new LinkedHashMap<>();
'JAVA TO VB CONVERTER NOTE: The field ops was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly ops_Conflict As IDictionary(Of String, SameDiffOp) = New LinkedHashMap(Of String, SameDiffOp)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final Map<Long, InferenceSession> sessions = new java.util.concurrent.ConcurrentHashMap<>();
		Private ReadOnly sessions As IDictionary(Of Long, InferenceSession) = New ConcurrentDictionary(Of Long, InferenceSession)() 'Key: thread ID
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private ArrayHolder constantArrays = new org.nd4j.autodiff.samediff.array.ThreadSafeArrayHolder(true);
		Private constantArrays As ArrayHolder = New ThreadSafeArrayHolder(True)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private ArrayHolder variablesArrays = new org.nd4j.autodiff.samediff.array.ThreadSafeArrayHolder(true);
		Private variablesArrays As ArrayHolder = New ThreadSafeArrayHolder(True)
		Private ReadOnly placeholdersPerThread As IDictionary(Of Long, IDictionary(Of String, INDArray)) = New ConcurrentDictionary(Of Long, IDictionary(Of String, INDArray))() 'Placeholders for each thread - if the user sets them

'JAVA TO VB CONVERTER NOTE: The field lossVariables was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly lossVariables_Conflict As IList(Of String) = New List(Of String)()

'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly listeners_Conflict As IList(Of Listener) = New List(Of Listener)()

		Private ReadOnly nameScopes As IList(Of NameScope) = New List(Of NameScope)() 'Used as a stack

'JAVA TO VB CONVERTER NOTE: The field outputs was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private outputs_Conflict As IList(Of String) 'Names of the output variables, set by the user.

		'/////////////////////////////////////
		'Fields related to training
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private TrainingConfig trainingConfig;
'JAVA TO VB CONVERTER NOTE: The field trainingConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private trainingConfig_Conflict As TrainingConfig 'Configuration for training. Must be set for training/evaluation, but not for other operations
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean initializedTraining;
		Private initializedTraining As Boolean 'True if training setup has been done
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private Map<String, org.nd4j.linalg.learning.GradientUpdater> updaterMap;
		Private updaterMap As IDictionary(Of String, GradientUpdater) 'GradientUpdater instance for each trainable parameter

		'//////////////////////////////////////

	'    private DifferentialFunctionFactory functionFactory;

		' counter for auto-naming variables
		Private variableId As Integer = 0

		'//////////////////////////////////////

		''' <summary>
		''' Op creator object for math operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field math was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly math_Conflict As New SDMath(Me)
		''' <summary>
		''' Op creator object for random number generation operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field random was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly random_Conflict As New SDRandom(Me)
		''' <summary>
		''' Op creator object for general neural network operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field nn was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly nn_Conflict As New SDNN(Me)
		''' <summary>
		''' Op creator object for convolutional neural network operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field cnn was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly cnn_Conflict As New SDCNN(Me)
		''' <summary>
		''' Op creator object for recurrent neural network operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field rnn was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly rnn_Conflict As New SDRNN(Me)
		''' <summary>
		''' Op creator object for loss function operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The variable loss was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public ReadOnly loss_Conflict As New SDLoss(Me)
		''' <summary>
		''' Op creator object for image operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field image was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly image_Conflict As New SDImage(Me)

		''' <summary>
		''' Op creator object for bitwise operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field bitwise was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly bitwise_Conflict As New SDBitwise(Me)

		''' <summary>
		''' Op creator object for linalg operations
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field linalg was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public ReadOnly linalg_Conflict As New SDLinalg(Me)

		''' <summary>
		''' Op creator object for math operations
		''' </summary>
		Public Overridable Function math() As SDMath
			Return math_Conflict
		End Function

		''' <summary>
		''' Op creator object for random number generation operations
		''' </summary>
		Public Overridable Function random() As SDRandom
			Return random_Conflict
		End Function

		''' <summary>
		''' Op creator object for general neural network operations
		''' </summary>
		Public Overridable Function nn() As SDNN
			Return nn_Conflict
		End Function

		''' <summary>
		''' Op creator object for convolutional neural network operations
		''' </summary>
		Public Overridable Function cnn() As SDCNN
			Return cnn_Conflict
		End Function

		''' <summary>
		''' Op creator object for recurrent neural network operations
		''' </summary>
		Public Overridable Function rnn() As SDRNN
			Return rnn_Conflict
		End Function

		''' <summary>
		''' Op creator object for loss function operations
		''' </summary>
		Public Overridable Function loss() As SDLoss
			Return loss_Conflict
		End Function

		''' <summary>
		''' Op creator object for image operations
		''' </summary>
		Public Overridable Function image() As SDImage
			Return image_Conflict
		End Function

		''' <summary>
		''' Op creator object for bitwise operations
		''' </summary>
		Public Overridable Function bitwise() As SDBitwise
			Return bitwise_Conflict
		End Function

		''' <summary>
		''' Op creator object for linalg operations
		''' </summary>
		Public Overridable Function linalg() As SDLinalg
			Return linalg_Conflict
		End Function

		Private sameDiffFunctionInstances As IDictionary(Of String, SameDiff)

		Private fieldVariableResolutionMapping As Table(Of String, String, String)

		' flag, shows if graph was already registered with libnd4j
		<NonSerialized>
		Private wasRegistered As New AtomicBoolean(False)


		'debug mode variables
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private boolean debugMode;
		Private debugMode As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private Stack<ArgumentInterceptor> argumentInterceptors = new Stack<>();
		Private argumentInterceptors As New Stack(Of ArgumentInterceptor)()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private @Set<ArgumentInterceptor> pausedArgumentInterceptors = new HashSet<>();
		Private pausedArgumentInterceptors As ISet(Of ArgumentInterceptor) = New HashSet(Of ArgumentInterceptor)()

		Private blockNames As ISet(Of String) = New HashSet(Of String)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter boolean logExecution = true;
		Friend logExecution As Boolean = True

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private SameDiff parent;
		Private parent As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private SameDiff child;
		Private child As SameDiff


		''' <summary>
		''' Clears debugging state and disables debug mode.
		''' </summary>
		Public Overridable Function disableDebugging() As SameDiff
			debugMode = False
			Return Me
		End Function

		''' <summary>
		''' Enables tracing of graphs automatically.
		''' </summary>
		Public Overridable Function enableDebugMode() As SameDiff
			debugMode = True
			Return Me
		End Function

		''' <summary>
		''' Set the current SameDiff-wide <seealso cref="Listener"/> instances.
		''' 
		''' Note that this will overwrite the current listener list.
		''' If you want to use additional listeners for a single operation,
		''' use the listener arguments in those methods (e.g. <seealso cref="fit()"/> and <seealso cref="FitConfig.listeners(Listener...)"/>).
		''' </summary>
		''' <param name="listeners"> Listeners </param>
		Public Overridable Property Listeners As Listener()
			Set(ByVal listeners() As Listener)
				Me.listeners_Conflict.Clear()
				addListeners(listeners)
			End Set
			Get
				Return listeners_Conflict
			End Get
		End Property

		''' <summary>
		''' See <seealso cref="setListeners(Listener...)"/>.
		''' </summary>
		Public Overridable WriteOnly Property Listeners(Of T1 As Listener) As ICollection(Of T1)
			Set(ByVal listeners As ICollection(Of T1))
				Me.listeners_Conflict.Clear()
				addListeners(listeners)
			End Set
		End Property


		''' <summary>
		''' Add SameDiff-wide <seealso cref="Listener"/> instances.
		''' 
		''' If you want to use additional listeners for a single operation,
		''' use the listener arguments in those methods (e.g. <seealso cref="fit()"/> and <seealso cref="FitConfig.listeners(Listener...)"/>).
		''' </summary>
		''' <param name="listeners"> Listeners </param>
		Public Overridable Sub addListeners(ParamArray ByVal listeners() As Listener)
			addListeners(java.util.Arrays.asList(listeners))
		End Sub

		''' <summary>
		''' See <seealso cref="addListeners(Listener...)"/>.
		''' </summary>
		Public Overridable Sub addListeners(Of T1 As Listener)(ByVal listeners As ICollection(Of T1))
			CType(Me.listeners_Conflict, List(Of Listener)).AddRange(listeners)
		End Sub


		''' <summary>
		''' Set the array holders for variable and constant arrays<br>
		''' <b>NOTE:</b> this is usually reserved for developers and internal use, and should not be needed by almost all users<br>
		''' See <seealso cref="ArrayHolder"/> for more details
		''' </summary>
		''' <param name="variableArrayHolder"> Array holder for variable arrays </param>
		''' <param name="constantArrayHolder"> Array holder for constant arrays </param>
		''' <param name="initialize">          If true: transfer any arrays from the current array holders to the new/specified ones </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setArrayHolders(@NonNull ArrayHolder variableArrayHolder, @NonNull ArrayHolder constantArrayHolder, boolean initialize)
		Public Overridable Sub setArrayHolders(ByVal variableArrayHolder As ArrayHolder, ByVal constantArrayHolder As ArrayHolder, ByVal initialize As Boolean)
			If initialize Then
				variableArrayHolder.initFrom(Me.variablesArrays)
				constantArrayHolder.initFrom(Me.constantArrays)
			End If
			Me.variablesArrays = variableArrayHolder
			Me.constantArrays = constantArrayHolder
		End Sub

		''' <returns> The current name scope, if any (null otherwise). See <seealso cref="withNameScope(String)"/> for more details. </returns>
		Public Overridable Function currentNameScope() As String
			If nameScopes.Count = 0 Then
				Return Nothing
			End If

			'Would use String.join but that is Java 8+
			Dim sb As New StringBuilder()
			Dim first As Boolean = True
			For Each ns As NameScope In nameScopes
				If Not first Then
					sb.Append("/")
				End If
				sb.Append(ns.getName())
				first = False
			Next ns
			Return sb.ToString()
		End Function

		''' <returns> The name with the current name scope (if any) appended. See <seealso cref="withNameScope(String)"/> </returns>
		Protected Friend Overridable Function nameWithScope(ByVal name As String) As String
			Dim scope As String = currentNameScope()
			If scope Is Nothing Then
				Return name
			End If
			If Not name.StartsWith(scope & "/", StringComparison.Ordinal) Then
				Return scope & "/" & name
			Else
				Return name
			End If
		End Function

		'Intentionally package private
		Friend Overridable Sub addNameScope(ByVal nameScope As NameScope)
			nameScopes.Add(nameScope)
		End Sub

		'Intentionally package private
		Friend Overridable Sub closeNameScope(ByVal nameScope As NameScope)
			'Check that the name scope is closed correctly/in order
			Preconditions.checkState(nameScopes.Count > 0, "Cannot close name scope: no name scopes are currently defined")
			Preconditions.checkState(nameScopes(nameScopes.Count - 1).Equals(nameScope), "Cannot close name scope %s: Name scopes must be closed in order. Current name scopes: ""%s""", nameScope, currentNameScope())

			nameScopes.RemoveAt(nameScopes.Count - 1)
		End Sub

		''' <summary>
		''' Create a name scope. Name scopes append a prefix to the names of any variables and ops created while they are open.
		''' <pre>
		'''  {@code
		'''  SameDiff sd = SameDiff.create();
		'''  SDVariable x = sd.var("x", DataType.FLOAT, 5);
		'''  SDVariable y;
		'''  try(NameScope ns = sd.withNameScope("myScope"){
		'''      y = sd.var("y", DataType.FLOAT, 5);
		'''  }
		'''  SDVariable z = sd.var("z", DataType.FLOAT, 5);
		''' 
		'''  String xName = x.name();      //RESULT: "x"
		'''  String yName = y.name();      //RESULT: "myScope/y"
		'''  String zName = z.name();      //RESULT: "z"
		'''  }
		''' </pre>
		''' <para>
		''' Note that name scopes can also be nested:
		''' <pre>
		'''  {@code
		'''  SameDiff sd = SameDiff.create();
		'''  SDVariable x;
		'''  try(NameScope ns = sd.withNameScope("first"){
		'''      try(NameScope ns2 = sd.withNameScope("second"){
		'''          x = sd.var("x", DataType.FLOAT, 5);
		'''      }
		'''  }
		'''  String xName = x.name();      //RESULT: "first/second/x"
		'''  }
		''' </pre>
		''' 
		''' </para>
		''' </summary>
		''' <param name="nameScope"> Name of the name scope to open/create </param>
		''' <returns> The NameScope object </returns>
		Public Overridable Function withNameScope(ByVal nameScope As String) As NameScope
			Dim ns As New NameScope(Me, nameScope)
			addNameScope(ns)
			Return ns
		End Function


		''' <summary>
		''' Gets all operations in a given name scope.
		''' </summary>
		Public Overridable Function getOpsInScope(ByVal scope As NameScope) As IList(Of SameDiffOp)
			Dim ops As New List(Of SameDiffOp)()
			For Each v As SameDiffOp In Me.ops_Conflict.Values
				If v.Name.StartsWith(scope.getName(), StringComparison.Ordinal) Then
					ops.Add(v)
				End If
			Next v
			Return ops
		End Function

		''' <summary>
		''' See <seealso cref="getOpsInScope(NameScope)"/>.
		''' </summary>
		Public Overridable Function getOpsInScope(ByVal scope As String) As IList(Of SameDiffOp)
			Return getOpsInScope(New NameScope(Me, scope))
		End Function

		''' <summary>
		''' Gets all variables in a given name scope.
		''' </summary>
		Public Overridable Function getVariablesInScope(ByVal scope As NameScope) As IList(Of SDVariable)
			Dim vars As New List(Of SDVariable)()
			For Each v As SDVariable In variables()
				If v.name().StartsWith(scope.getName(), StringComparison.Ordinal) Then
					vars.Add(v)
				End If
			Next v
			Return vars
		End Function

		''' <summary>
		''' See <seealso cref="getVariablesInScope(NameScope)"/>.
		''' </summary>
		Public Overridable Function getVariablesInScope(ByVal scope As String) As IList(Of SDVariable)
			Return getVariablesInScope(New NameScope(Me, scope))
		End Function

		''' <param name="sameDiff">
		''' @return </param>
'JAVA TO VB CONVERTER NOTE: The parameter sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function invokeGraphOn(ByVal sameDiff_Conflict As SameDiff) As SDVariable
			'map the new vertices on to the old ones
			Dim thisVertexIdToNew As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim idx As Integer = 1
			For Each var As val In variables()
				Dim clone As SDVariable = var.clone(Me)
				Dim newVar As SDVariable = sameDiff_Conflict.var(clone)
				If var.getVariableType() <> VariableType.ARRAY AndAlso var.getArr() IsNot Nothing Then 'ARRAY type = "activations" - are overwritten anyway
					sameDiff_Conflict.associateArrayWithVariable(var.getArr(), newVar)
				End If


				thisVertexIdToNew(idx) = idx
				clone.setSameDiff(sameDiff_Conflict)
				idx += 1

			Next var

			Dim reverseMap As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim count As Integer = 0
			For Each v As Variable In variables_Conflict.Values
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: reverseMap.put(v.getName(), count++);
				reverseMap(v.getName()) = count
					count += 1
			Next v

			Dim newFunctions As val = New LinkedHashMap(Of String, DifferentialFunction)()
			For Each op As SameDiffOp In ops_Conflict.Values
				Dim [function] As DifferentialFunction = op.Op

				'Clone the op
				Dim clone As DifferentialFunction = FlatBuffersMapper.cloneViaSerialize(Me, [function], reverseMap)

				clone.setSameDiff(sameDiff_Conflict)
				clone.setOwnName([function].getOwnName())
				If sameDiff_Conflict.opExists([function].getOwnName()) Then
					sameDiff_Conflict.putOpForId([function].getOwnName(), [function])
				End If
				newFunctions.put([function].getOwnName(), clone)

				Dim argsForFunction As val = [function].args()
				Dim outputsForFunction As val = [function].outputVariables()

				'note that these have the same variable names
				sameDiff_Conflict.addArgsFor(argsForFunction, clone)
				sameDiff_Conflict.addOutgoingFor(outputsForFunction, [function])

				For Each arg As val In clone.args()
					arg.setSameDiff(sameDiff_Conflict)
				Next arg

				For Each output As val In clone.outputVariables()
					output.setSameDiff(sameDiff_Conflict)
				Next output

				sameDiff_Conflict.ops_Conflict([function].getOwnName()) = op
			Next op

			Return sameDiff_Conflict.variables()(sameDiff_Conflict.variables().Count - 1)
		End Function


		''' <summary>
		''' Returns true if the given function id exists
		''' </summary>
		''' <param name="id"> the function id to test for </param>
		''' <returns> true if the function id exists, false otherwise </returns>
		Public Overridable Function opExists(ByVal id As String) As Boolean
			Return ops_Conflict.ContainsKey(id)
		End Function

		''' <summary>
		''' Get the differential function (if any) that this variable is the output for
		''' </summary>
		''' <param name="variableName"> Name of the variable </param>
		''' <returns> The differential function that this variable is an output of, or null if it is not the output of a function </returns>
		Public Overridable Function getVariableOutputOp(ByVal variableName As String) As DifferentialFunction
			Preconditions.checkState(variables_Conflict.ContainsKey(variableName), "No variable with name ""%s"" found in graph", variableName)
			If variables(variableName).getOutputOfOp() Is Nothing OrElse ops(stripVarSuffix(variables(variableName).getOutputOfOp())) Is Nothing Then
				Return Nothing
			End If
			Return ops(stripVarSuffix(variables(variableName).getOutputOfOp())).getOp()
		End Function

		''' <summary>
		''' Get the function by the <seealso cref="DifferentialFunction.getOwnName()"/>
		''' </summary>
		''' <param name="id"> the id of the function </param>
		''' <returns> the function for the given id if it exists </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.functions.DifferentialFunction getOpById(@NonNull String id)
		Public Overridable Function getOpById(ByVal id As String) As DifferentialFunction
			If Not ops_Conflict.ContainsKey(id) Then
				Throw New ND4JIllegalStateException("No function with id " & id & " found!")
			End If
			Return ops(id).getOp()
		End Function


		''' <summary>
		''' Put the function for the given id
		''' </summary>
		''' <param name="id">       the id of the function </param>
		''' <param name="function"> the function </param>
		Public Overridable Sub putOpForId(ByVal id As String, ByVal [function] As DifferentialFunction)
			If ops_Conflict.ContainsKey(id) AndAlso ops(id).getOp() Is Nothing Then
				Throw New ND4JIllegalStateException("Function by id already exists!")
			End If

			If Not ops_Conflict.ContainsKey(id) Then
				ops(id) = SameDiffOp.builder().name(id).op([function]).build()
			End If
		End Sub


		''' <summary>
		''' Returns the name(s) of the inputs for the given function
		''' </summary>
		''' <param name="function"> the function to get the inputs for </param>
		''' <returns> the input ids for a given function </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public String[] getInputsForOp(@NonNull DifferentialFunction function)
		Public Overridable Function getInputsForOp(ByVal [function] As DifferentialFunction) As String()
			If Not ops_Conflict.ContainsKey([function].getOwnName()) Then
				Throw New ND4JIllegalStateException("Unknown function instance id found: """ & [function].getOwnName() & """")
			End If
			Dim inputs As IList(Of String) = ops([function].getOwnName()).getInputsToOp()
			Return If(inputs Is Nothing, Nothing, CType(inputs, List(Of String)).ToArray())
		End Function

		''' <summary>
		''' Returns the name(s) of the outputs for the given function
		''' </summary>
		''' <param name="function"> the function to get the outputs for </param>
		''' <returns> the outputs ids for a given function </returns>
		Public Overridable Function getOutputsForOp(ByVal [function] As DifferentialFunction) As String()
			If Not ops_Conflict.ContainsKey([function].getOwnName()) Then
				Throw New ND4JIllegalStateException("Illegal function instance id found " & [function].getOwnName())
			End If
			Dim outputs As IList(Of String) = ops([function].getOwnName()).getOutputsOfOp()
			Return If(outputs Is Nothing, Nothing, CType(outputs, List(Of String)).ToArray())
		End Function


		''' <summary>
		''' Get the output variable(s) for the specified differential function
		''' </summary>
		''' <param name="function"> the function reference to get the output variable(s) for </param>
		''' <returns> the output variables for the given function </returns>
		Public Overridable Function getOutputVariablesForOp(ByVal [function] As DifferentialFunction) As SDVariable()
			Dim inputs As val = getOutputsForOp([function])
			If inputs Is Nothing Then
				Throw New ND4JIllegalStateException("No inputs found for function " & [function])
			End If

			Dim vars As val = New SDVariable(inputs.length - 1){}
			For i As Integer = 0 To inputs.length - 1
				vars(i) = getVariable(inputs(i))
			Next i

			Return vars
		End Function


		''' <summary>
		''' Get the input variable(s) for the specified differential function
		''' </summary>
		''' <param name="function"> the function reference to get the input variable(s) for </param>
		''' <returns> the input variables for the given function </returns>
		Public Overridable Function getInputVariablesForOp(ByVal [function] As DifferentialFunction) As SDVariable()
			Dim inputs As val = getInputsForOp([function])
			If inputs Is Nothing Then
				Throw New ND4JIllegalStateException("No inputs found for function " & [function])
			End If

			Dim vars As val = New SDVariable(inputs.length - 1){}
			For i As Integer = 0 To inputs.length - 1
				vars(i) = getVariable(inputs(i))
				If vars(i) Is Nothing Then
					Throw New ND4JIllegalStateException("Found null variable at index " & i)
				End If
			Next i

			Return vars
		End Function


		''' <summary>
		''' Set the stored <seealso cref="INDArray"/> for a variable.  Only works if the variable is of type
		''' <seealso cref="VariableType.CONSTANT"/>, <seealso cref="VariableType.PLACEHOLDER"/>, or <seealso cref="VariableType.VARIABLE"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setArrayForVariable(@NonNull String varName, @NonNull INDArray arr)
		Public Overridable Sub setArrayForVariable(ByVal varName As String, ByVal arr As INDArray)
			Preconditions.checkState(variables_Conflict.ContainsKey(varName), "No variable with name ""%s"" exists", varName)

			Dim v As SDVariable = getVariable(varName)
			If v.Constant Then
				constantArrays.setArray(varName, arr)
			ElseIf v.getVariableType() = VariableType.VARIABLE Then
				variablesArrays.setArray(varName, arr)
			ElseIf v.PlaceHolder Then
				Dim tid As Long = Thread.CurrentThread.getId()
				If Not placeholdersPerThread.ContainsKey(tid) Then
					placeholdersPerThread(tid) = New Dictionary(Of String, INDArray)()
				End If
				placeholdersPerThread(tid)(varName) = arr
			Else
				Throw New System.NotSupportedException("Cannot set variable of type " & v.getVariableType() & " using this method")
			End If
		End Sub


		''' <summary>
		''' Returns true if the given vertex id and <seealso cref="INDArray"/> already exist.
		''' </summary>
		''' <param name="varName"> the vertex id </param>
		''' <returns> true if a vertex with the given INDArray exists, and it has an INDArray associated with it </returns>
		Public Overridable Function arrayAlreadyExistsForVarName(ByVal varName As String) As Boolean
			Dim var As SDVariable = getVariable(varName)
			Select Case var.getVariableType()
				Case VARIABLE
					Return variablesArrays.hasArray(varName)
				Case ARRAY
					Dim tid As Long = Thread.CurrentThread.getId()
					Return sessions.ContainsKey(tid) AndAlso sessions(tid).contains(varName, InferenceSession.OUTER_FRAME, 0, Nothing)
				Case CONSTANT
					Return constantArrays.hasArray(varName)
				Case PLACEHOLDER
					Return placeholdersPerThread.ContainsKey(Thread.CurrentThread.getId()) AndAlso placeholdersPerThread(Thread.CurrentThread.getId()).ContainsKey(varName)
				Case Else
					Throw New Exception("Unknown variable type: " & var.getVariableType())
			End Select
		End Function

		''' <summary>
		''' Get an <seealso cref="INDArray"/> for a given vertex id, or null if none exists
		''' </summary>
		''' <param name="varName"> Variable name to get the array for </param>
		''' <returns> Array, or null if none exists </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray getArrForVarName(@NonNull String varName)
		Public Overridable Function getArrForVarName(ByVal varName As String) As INDArray
			Preconditions.checkState(variables_Conflict.ContainsKey(varName), "No variable found with name ""%s""", varName)
			Dim v As SDVariable = variables(varName).getVariable()
			Select Case v.getVariableType()
				Case VARIABLE
					Return variablesArrays.getArray(varName)
				Case CONSTANT
					If Not constantArrays.hasArray(varName) Then
						Return Nothing
					End If
					Return constantArrays.getArray(varName)
				Case ARRAY
					'Only stored in inference session...
					Dim s As InferenceSession = sessions(Thread.CurrentThread.getId())
					If s Is Nothing Then
						Throw New System.NotSupportedException("Cannot get array for ARRAY type SDVariable - use SDVariable.exec or SameDiff.output instead")
					End If

					Return s.get(varName, InferenceSession.OUTER_FRAME, 0, Nothing, False)
				Case PLACEHOLDER
					Dim tid As Long = Thread.CurrentThread.getId()
					If placeholdersPerThread(tid) Is Nothing OrElse Not placeholdersPerThread(tid).ContainsKey(varName) Then
						Return Nothing
					End If
					Return placeholdersPerThread(tid)(varName)
				Case Else
					Throw New Exception("Unknown variable type: " & v.getVariableType())
			End Select
		End Function

		''' <summary>
		''' Associate the array with the given variable.
		''' </summary>
		''' <param name="arr">      the array to get the variable for </param>
		''' <param name="variable"> the name of the variable to associate the array with </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void associateArrayWithVariable(org.nd4j.linalg.api.ndarray.INDArray arr, @NonNull String variable)
		Public Overridable Sub associateArrayWithVariable(ByVal arr As INDArray, ByVal variable As String)
			Preconditions.checkState(variables_Conflict.ContainsKey(variable), "Cannot associate array with variable ""%s"": " & "variable ""%s"" does not exist in this SameDiff instance", variable, variable)
			associateArrayWithVariable(arr, Me.getVariable(variable))
		End Sub

		''' <summary>
		''' Associate the array with the given variable.
		''' </summary>
		''' <param name="arr">      the array to get the variable for </param>
		''' <param name="variable"> the variable to associate the array with </param>
		Public Overridable Sub associateArrayWithVariable(ByVal arr As INDArray, ByVal variable As SDVariable)
			If variable Is Nothing Then
				Throw New ND4JIllegalArgumentException("Variable must not be null!")
			End If
			If arr Is Nothing Then
				Throw New ND4JIllegalArgumentException("Array must not be null")
			End If

			If variable.dataType() <> arr.dataType() Then
				arr = arr.castTo(variable.dataType())
			End If

			Preconditions.checkState(variable.dataType() = arr.dataType(), "Variable ""%s"" has datatype %s: cannot associate array with type %s with this variable", variable.name(), variable.dataType(), arr.dataType())

			If sessions(Thread.CurrentThread.getId()) Is Nothing Then
				sessions(Thread.CurrentThread.getId()) = New InferenceSession(Me)
			End If

			If arr.Attached Then
				arr = arr.detach()
			End If

			Select Case variable.getVariableType()
				Case VARIABLE
					variablesArrays.setArray(variable.name(), arr)
				Case CONSTANT
					constantArrays.setArray(variable.name(), arr)
				Case ARRAY
					Throw New System.NotSupportedException("Cannot associate array with SDVariable of type ARRAY - arrays for" & " this type of variable is calculated ")
				Case PLACEHOLDER
					'Validate placeholder shapes:
					Dim phShape() As Long = variable.placeholderShape()
					Preconditions.checkState(phShape Is Nothing OrElse Shape.shapeMatchesPlaceholder(phShape, arr.shape()), "Invalid array shape: cannot associate an array with shape %ndShape with a placeholder of shape %s:" & "shape is wrong rank or does not match on one or more dimensions", arr, phShape)


					Dim tid As Long = Thread.CurrentThread.getId()
					If Not placeholdersPerThread.ContainsKey(tid) Then
						placeholdersPerThread(tid) = New Dictionary(Of String, INDArray)()
					End If
					placeholdersPerThread(tid)(variable.name()) = arr
				Case Else
					Throw New System.InvalidOperationException("Unknown variable type: " & variable.getVariableType())
			End Select

			'putOrUpdateShapeForVarName(variable.name(), arr.shape(), true);

			'Also update nested SameDiff instances (such as gradient function)
			If sameDiffFunctionInstances IsNot Nothing AndAlso sameDiffFunctionInstances.Count > 0 Then
				For Each e As KeyValuePair(Of String, SameDiff) In sameDiffFunctionInstances.SetOfKeyValuePairs()
					Dim sd As SameDiff = e.Value
					Dim v As SDVariable = sd.getVariable(variable.name())
					If v IsNot Nothing Then
						sd.associateArrayWithVariable(arr, v)
					End If
				Next e
			End If
		End Sub

		''' <summary>
		''' Update the constant or variable type SDVariable with the values from the specified
		''' array. Note that unlike <seealso cref="associateArrayWithVariable(INDArray, String)"/> this method will take the
		''' values from the argument array and assign it to the current array.
		''' The actual array (INDArray object) will not be stored or otherwise used within the SameDiff instance. </summary>
		''' <param name="arr">      Array values to set </param>
		''' <param name="variable"> Variable to update the array of. Must be CONSTANT or VARIBLE type SDVariable </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void assignArray(@NonNull INDArray arr, @NonNull SDVariable variable)
		Public Overridable Sub assignArray(ByVal arr As INDArray, ByVal variable As SDVariable)
			Preconditions.checkState(variable.getVariableType() = VariableType.VARIABLE OrElse variable.getVariableType() = VariableType.CONSTANT, "assignArray method can only be used with VARIBLE or CONSTANT type SDVariables, variable ""%s"" has type %s", variable.name(), variable.getVariableType())

			'DeviceLocal doesn't work with views
			If arr.isView() Then
				arr = arr.dup()
			End If

			If variable.getVariableType() = VariableType.VARIABLE Then
				variablesArrays.setArray(variable.name(), arr)
			Else
				constantArrays.setArray(variable.name(), arr)
			End If
		End Sub


		''' <summary>
		''' Associate a <seealso cref="SameDiff"/> namespace as a sub function.
		''' </summary>
		''' <param name="name">      the opName of the function </param>
		''' <param name="nameSpace"> the namespace </param>
		Public Overridable Sub putSubFunction(ByVal name As String, ByVal [nameSpace] As SameDiff)
			If sameDiffFunctionInstances.ContainsKey(name) AndAlso sameDiffFunctionInstances(name) IsNot [nameSpace] Then
				Throw New ND4JIllegalStateException("Unable to replace samediff namespace. Please choose another opName")
			End If

			sameDiffFunctionInstances(name) = [nameSpace]
		End Sub


		''' <summary>
		''' Return a copy of the internal variable map
		''' </summary>
		''' <returns> Map of variables by name </returns>
		Public Overridable Function variableMap() As IDictionary(Of String, SDVariable)
			Dim ret As IDictionary(Of String, SDVariable) = New LinkedHashMap(Of String, SDVariable)()
			For Each v As Variable In variables_Conflict.Values
				ret(v.getName()) = v.getVariable()
			Next v
			Return ret
		End Function

		''' <summary>
		''' The set of defined SameDiff function names. SameDiff function instances should not be confused
		''' with DifferentialFunction ops; an example of a SameDiff function instance is the gradient "grad" function
		''' </summary>
		''' <returns> Set of defined SameDiff function instance names </returns>
		Public Overridable Function definedFunctionNames() As ICollection(Of String)
			Return Me.sameDiffFunctionInstances.Keys
		End Function

		Private Sub New()
			MyBase.New(Nothing)
			MyBase.sd = Me
			sameDiffFunctionInstances = New LinkedHashMap(Of String, SameDiff)()
			fieldVariableResolutionMapping = HashBasedTable.create()
		End Sub


		''' <summary>
		''' Attempts to insert the <seealso cref="DifferentialFunction"/> reference in to this <seealso cref="SameDiff"/> instance.
		''' If the given array field with the given index already exists, it will do a reference check to ensure that the 2
		''' array fields are the same. If not, an exception is thrown.<br>
		''' If the instances are the same (by semantics, not reference) then it will just return the original instance.
		''' This is to ensure that instances that are created are unique and reference checked.
		''' </summary>
		''' <param name="function"> the array field to attempt to create </param>
		''' <returns> Original instance </returns>
		Public Overridable Function setupFunction(Of X As SDVariable)(ByVal [function] As X) As X
			Preconditions.checkNotNull([function], "Passed in function must not be null!")
			If TypeOf [function] Is SDVariable Then
				If [function].getSameDiff() IsNot Me Then
					[function].setSameDiff(Me)
				End If
				Return [function]
			End If
			Return [function]
		End Function


		''' <summary>
		''' Adds outgoing arguments to the graph for the specified DifferentialFunction
		''' Also checks for input arguments and updates the graph adding an appropriate edge when the full graph is declared.
		''' </summary>
		''' <param name="variables"> Variables - arguments for the specified differential function </param>
		''' <param name="function">  Differential function </param>
		Public Overridable Sub addOutgoingFor(ByVal variables() As SDVariable, ByVal [function] As DifferentialFunction)
			Dim varNames(variables.Length - 1) As String
			For i As Integer = 0 To varNames.Length - 1
				varNames(i) = variables(i).name()
			Next i

			addOutgoingFor(varNames, [function])
		End Sub


		''' <summary>
		''' Adds outgoing arguments to the graph for the specified DifferentialFunction
		''' Also checks for input arguments and updates the graph adding an appropriate edge when the full graph is declared.
		''' </summary>
		''' <param name="varNames"> Name of the variables that are outputs of the specified differential function </param>
		''' <param name="function"> Differential function </param>
		Public Overridable Sub addOutgoingFor(ByVal varNames() As String, ByVal [function] As DifferentialFunction)

			If [function].getOwnName() Is Nothing Then
				Throw New ND4JIllegalStateException("Instance id can not be null. Function not initialized properly")
			End If


			If ops([function].getOwnName()).getOutputsOfOp() IsNot Nothing AndAlso Not ops([function].getOwnName()).getOutputsOfOp().isEmpty() Then
				Throw New ND4JIllegalStateException("Outgoing arguments already declared for " & [function])
			End If

			If varNames Is Nothing Then
				Throw New ND4JIllegalStateException("Var names can not be null!")
			End If


			For i As Integer = 0 To varNames.Length - 1
				If varNames(i) Is Nothing Then
					Throw New ND4JIllegalStateException("Variable name elements can not be null!")
				End If
			Next i

			ops([function].getOwnName()).setOutputsOfOp(java.util.Arrays.asList(varNames))

			For Each resultName As String In varNames
				variables(resultName).setOutputOfOp([function].getOwnName())
			Next resultName
		End Sub

		''' <summary>
		''' Add a new argument interceptor to the interceptor stack
		''' <para>
		''' For internal use only.
		''' </para>
		''' <para>
		''' When a op is added with arguments, most recent argument interceptor is called on it.
		''' If ops are added in that interceptor, the next most recent will be called on their args, and so on.
		''' 
		''' </para>
		''' </summary>
		''' <param name="interceptor"> the argument interceptor to add </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addArgumentInterceptor(@NonNull ArgumentInterceptor interceptor)
		Public Overridable Sub addArgumentInterceptor(ByVal interceptor As ArgumentInterceptor)
			argumentInterceptors.Push(interceptor)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private boolean isArgumentInterceptorPaused(@NonNull ArgumentInterceptor interceptor)
		Private Function isArgumentInterceptorPaused(ByVal interceptor As ArgumentInterceptor) As Boolean
			Return pausedArgumentInterceptors.Contains(interceptor)
		End Function

		Private ReadOnly Property ArgumentInterceptorToUse As ArgumentInterceptor
			Get
    
				If argumentInterceptors.Count = 0 Then
					Return Nothing
				End If
    
				Dim use As ArgumentInterceptor = argumentInterceptors.Peek()
				Dim i As Integer = 1
				Do While isArgumentInterceptorPaused(use)
					If argumentInterceptors.Count - i < 0 Then
						Return Nothing
					End If
    
	'JAVA TO VB CONVERTER TODO TASK: There is no direct .NET Stack equivalent to Java Stack methods based on internal indexing:
					use = argumentInterceptors.elementAt(argumentInterceptors.Count - i)
					i += 1
				Loop
    
				Return use
			End Get
		End Property

		''' <summary>
		''' Remote the top (most recently added) argument interceptor
		''' <para>
		''' For internal use only.
		''' </para>
		''' </summary>
		Public Overridable Sub removeArgumentInterceptor()
			If argumentInterceptors.Count > 0 Then
				argumentInterceptors.Pop()
			End If
		End Sub

		''' <summary>
		''' Pause the top (most recently added) argument interceptor
		''' <para>
		''' For internal use only.
		''' </para>
		''' </summary>
		Public Overridable Sub pauseArgumentInterceptor()
			pausedArgumentInterceptors.Add(argumentInterceptors.Peek())
		End Sub

		''' <summary>
		''' Pause the given argument interceptor
		''' <para>
		''' For internal use only.
		''' 
		''' </para>
		''' </summary>
		''' <param name="interceptor"> the argument interceptor to pause </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void pauseArgumentInterceptor(@NonNull ArgumentInterceptor interceptor)
		Public Overridable Sub pauseArgumentInterceptor(ByVal interceptor As ArgumentInterceptor)
			pausedArgumentInterceptors.Add(interceptor)
		End Sub

		''' <summary>
		''' Unpause the top (most recently added) argument interceptor
		''' <para>
		''' For internal use only.
		''' </para>
		''' </summary>
		Public Overridable Sub unpauseArgumentInterceptor()
			pausedArgumentInterceptors.remove(argumentInterceptors.Peek())
		End Sub

		''' <summary>
		''' Unpause the top given argument interceptor
		''' <para>
		''' For internal use only.
		''' 
		''' </para>
		''' </summary>
		''' <param name="interceptor"> the argument interceptor to unpause </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void unpauseArgumentInterceptor(@NonNull ArgumentInterceptor interceptor)
		Public Overridable Sub unpauseArgumentInterceptor(ByVal interceptor As ArgumentInterceptor)
			pausedArgumentInterceptors.remove(interceptor)
		End Sub

		''' <summary>
		''' Adds incoming arguments for the specified differential function to the graph
		''' </summary>
		''' <param name="variables"> Name of the variables that are arguments (inputs) to the specified function </param>
		''' <param name="function">  Function </param>
		Public Overridable Sub addArgsFor(ByVal variables() As String, ByVal [function] As DifferentialFunction)

			Dim interceptor As ArgumentInterceptor = ArgumentInterceptorToUse

			If interceptor IsNot Nothing Then
				pauseArgumentInterceptor(interceptor)
				For i As Integer = 0 To variables.Length - 1
					variables(i) = interceptor.intercept(getVariable(variables(i))).name()
				Next i
				unpauseArgumentInterceptor(interceptor)
			End If

			If [function].getOwnName() Is Nothing Then
				Throw New ND4JIllegalStateException("Instance id can not be null. Function not initialized properly")
			End If

			'Add function if it doesn't exist
			'TODO could "not existing" be a bug sometimes?
			If Not ops_Conflict.ContainsKey([function].getOwnName()) Then
				ops([function].getOwnName()) = SameDiffOp.builder().name([function].getOwnName()).op([function]).build()
			End If

			'Update variable 'inputs to op' accounting for repeated inputs (like y = x+x)
			ops([function].getOwnName()).setInputsToOp(java.util.Arrays.asList(variables)) 'Duplicate variables OK/required here

			For Each variableName As String In variables
				If Me.variables_Conflict.ContainsKey(variableName) Then
					Dim funcs As IList(Of String) = Me.variables_Conflict(variableName).getInputsForOp()
					If funcs Is Nothing Then
						funcs = New List(Of String)()
						Me.variables_Conflict(variableName).setInputsForOp(funcs)
					End If
					If Not funcs.Contains([function].getOwnName()) Then 'Avoid duplicates for function names.
						funcs.Add([function].getOwnName())
					End If
				End If

			Next variableName
		End Sub

		''' <summary>
		''' Adds incoming arguments for the specified differential function to the graph
		''' </summary>
		''' <param name="variables"> variables that are arguments (inputs) to the specified function </param>
		''' <param name="function">  Function </param>
		Public Overridable Sub addArgsFor(ByVal variables() As SDVariable, ByVal [function] As DifferentialFunction)

			Dim varNames(variables.Length - 1) As String
			For i As Integer = 0 To varNames.Length - 1
				If variables(i) Is Nothing Then
					Throw New ND4JIllegalStateException("Found null variable at index " & i)
				End If
				varNames(i) = variables(i).name()
			Next i
			addArgsFor(varNames, [function])
		End Sub

		''' <summary>
		''' Replaces the argument at i with newArg for function
		''' Does not use (or remove) ArgumentInterceptor stuff
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void replaceArgFor(int i, @NonNull SDVariable newArg, @NonNull DifferentialFunction function)
		Public Overridable Sub replaceArgFor(ByVal i As Integer, ByVal newArg As SDVariable, ByVal [function] As DifferentialFunction)

			Preconditions.checkArgument(i < [function].args().length, "Index out of range: function " & [function].getOwnName() & " only has " & [function].args().length & " args but you are trying" & "to replace the argument at " & i)

			Dim oldName As String = [function].arg(i).name()
			Dim newName As String = newArg.name()

			Dim oldArgs As IList(Of String) = ops([function].getOwnName()).getInputsToOp()
			oldArgs = New List(Of String)(oldArgs)
			oldArgs(i) = newName
			ops([function].getOwnName()).setInputsToOp(oldArgs)

			Dim funcs As IList(Of String) = Me.variables_Conflict(newName).getInputsForOp()

			If funcs Is Nothing Then
				funcs = New List(Of String)()
				Me.variables_Conflict(newName).setInputsForOp(funcs)
			End If
			If Not funcs.Contains([function].getOwnName()) Then 'Avoid duplicates for function names.
				funcs.Add([function].getOwnName())
			End If

			Dim oldFuncs As IList(Of String) = Me.variables_Conflict(oldName).getInputsForOp()
			If oldFuncs IsNot Nothing Then
				If Not ArrayUtils.contains([function].argNames(), oldName) Then
					oldFuncs.Remove([function].getOwnName())
				End If
			End If

		End Sub


		''' <summary>
		''' Returns true if this function already has defined arguments
		''' </summary>
		''' <param name="function"> the function to check </param>
		''' <returns> true if the function has args, false otherwise </returns>
		Public Overridable Function hasArgs(ByVal [function] As DifferentialFunction) As Boolean
			Dim vertexIdArgs As IList(Of String) = ops([function].getOwnName()).getInputsToOp()
			Return vertexIdArgs IsNot Nothing AndAlso vertexIdArgs.Count > 0
		End Function

		''' <summary>
		''' Clear the placeholder arrays from the SameDiff instance
		''' </summary>
		''' <param name="allThreads"> If true: clear the placeholders for all threads. False: clear only for current thread </param>
		Public Overridable Sub clearPlaceholders(ByVal allThreads As Boolean)
			If allThreads Then
				Me.placeholdersPerThread.Clear()
			Else
				Dim tid As Long = Thread.CurrentThread.getId()
				Me.placeholdersPerThread.Remove(tid)
			End If
			For Each sd As SameDiff In Me.sameDiffFunctionInstances.Values
				sd.clearPlaceholders(allThreads)
			Next sd
		End Sub

		''' <summary>
		''' Clear the input arrays to each op.
		''' This is usually not required, under normal SameDiff use
		''' </summary>
		Public Overridable Sub clearOpInputs()
			For Each op As SameDiffOp In ops_Conflict.Values
				If TypeOf op.Op Is Op Then
					Dim o As Op = (DirectCast(op.Op, Op))
					o.X = Nothing
					If o.y() IsNot Nothing Then
						o.Y = Nothing
					End If
				ElseIf TypeOf op.Op Is DynamicCustomOp Then
					Dim o As DynamicCustomOp = DirectCast(op.Op, DynamicCustomOp)
					o.InputArguments = DirectCast(Nothing, INDArray())
				End If
			Next op
			For Each sd As SameDiff In Me.sameDiffFunctionInstances.Values
				sd.clearOpInputs()
			Next sd
		End Sub

		''' <summary>
		''' Get an array of differential functions that have been defined for this SameDiff instance
		''' </summary>
		''' <returns> Array of differential functions </returns>
		Public Overridable Function ops() As DifferentialFunction()
			Dim [out] As IList(Of DifferentialFunction) = New List(Of DifferentialFunction)(ops_Conflict.Count)
			For Each op As SameDiffOp In ops_Conflict.Values
				[out].Add(op.Op)
			Next op
			Return CType([out], List(Of DifferentialFunction)).ToArray()
		End Function


		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = MyBase.GetHashCode()
			result = 31 * result + (If(variables_Conflict IsNot Nothing, variables_Conflict.GetHashCode(), 0))
			Return result
		End Function

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

'JAVA TO VB CONVERTER NOTE: The variable sameDiff was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim sameDiff_Conflict As SameDiff = DirectCast(o, SameDiff)

			Dim eqVars As Boolean = variables_Conflict.Equals(sameDiff_Conflict.variables_Conflict)
			Dim eqOps As Boolean = ops_Conflict.Equals(sameDiff_Conflict.ops_Conflict)
			Return eqVars AndAlso eqOps
		End Function

		''' <summary>
		''' Create a new (empty) SameDiff instance without any functions or variables
		''' </summary>
		''' <returns> New SameDiff instance </returns>
		Public Shared Function create() As SameDiff
			Return New SameDiff()
		End Function

		''' <summary>
		''' Clone/duplicate the SameDiff instance, including arrays etc. The returned SameDiff instance should have no
		''' shared state with the original instance
		''' </summary>
		''' <returns> The cloned SameDiff instance </returns>
		Public Overridable Function dup() As SameDiff
			Dim bb As ByteBuffer = asFlatBuffers(True)
			Try
				Return fromFlatBuffers(bb)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function


		''' <summary>
		''' Count the number of elements in all arrays, according to <seealso cref="SDVariable.getShape()"/>
		''' </summary>
		''' <returns> Number of array elements for all variables </returns>
		Public Overridable Function numElements() As Long
			Dim ret As Long = 0
			For Each variable As SDVariable In variables()
				Dim shape() As Long = variable.Shape
				If shape IsNot Nothing Then
					ret += ArrayUtil.prod(shape)
				End If
			Next variable
			Return ret
		End Function

		''' <summary>
		''' Returns the inputs (placeholders) for the SameDiff graph
		''' </summary>
		''' <returns> the inputs for this graph </returns>
		Public Overridable Function inputs() As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each s As String In variables_Conflict.Keys
				If isPlaceHolder(s) Then
					[out].Add(s)
				End If
			Next s
			Return [out]
		End Function

		''' <summary>
		''' Outputs are the names of the predictions of the network.
		''' Note that the outputs must be set using <seealso cref="setOutputs(List)"/> first
		''' </summary>
		''' <returns> The outputs of the SameDiff instance, or null if no outputs have been set </returns>
		Public Overridable Function outputs() As IList(Of String)
			Return Me.outputs_Conflict
		End Function

		''' <summary>
		''' See <seealso cref="setOutputs(List)"/>
		''' </summary>
		Public Overridable WriteOnly Property Outputs As String()
			Set(ByVal outputs() As String)
				setOutputs(If(outputs Is Nothing, Nothing, java.util.Arrays.asList(outputs)))
			End Set
		End Property


		''' <summary>
		''' Set the outputs of the SameDiff instance.
		''' Outputs are the names of the variables that are the predictions of the neural network.
		''' Note that this is merely a convenience, and does not impact execution at all. Outputs can be retrieved (after
		''' setting here) using <seealso cref="outputs()"/> </summary>
		''' <param name="outputs"> Outputs to set. Must be valid variable names in this SameDiff instance </param>
		Public Overridable WriteOnly Property Outputs As IList(Of String)
			Set(ByVal outputs As IList(Of String))
				If outputs IsNot Nothing Then
					For Each s As String In outputs
						Preconditions.checkArgument(variables_Conflict.ContainsKey(s), "Cannot set variable ""%s"" as an output: SameDiff instance does not contain a variable with this name")
					Next s
				End If
				Me.outputs_Conflict = outputs
			End Set
		End Property

		''' <summary>
		''' The list of all variables in the graph
		''' </summary>
		''' <returns> All variables in the graph </returns>
		Public Overridable Function variables() As IList(Of SDVariable)
			Return New List(Of SDVariable)(variableMap().Values)
		End Function


		''' <summary>
		''' Get the names of variables (if any) that have been marked as loss variables to be minimized.<br>
		''' Variables can be marked as loss variables in a few different ways:<br>
		''' (a) Losses are automatically added when creating loss functions via <seealso cref="sd()"/><br>
		''' (b) Via <seealso cref="setLossVariables(String...)"/>, @link #addLossVariable(String)} or <seealso cref="SDVariable.markAsLoss()"/><br>
		''' (c) Via <seealso cref="TrainingConfig.setLossVariables(List)"/><br>
		''' </summary>
		Public Overridable Property LossVariables As IList(Of String)
			Get
				Return Collections.unmodifiableList(Me.lossVariables_Conflict)
			End Get
			Set(ByVal lossVariableNames() As String)
				Me.lossVariables_Conflict.Clear()
				For Each s As String In lossVariableNames
					addLossVariable(s)
				Next s
				'After changing loss function variables, we (probably) need to recreate gradient function - as gradient
				' function is defined with respect to specific loss function variables
				sameDiffFunctionInstances.Remove("grad")
			End Set
		End Property


		''' <summary>
		''' See <seealso cref="setLossVariables(String...)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setLossVariables(@NonNull SDVariable... lossVariables)
		Public Overridable WriteOnly Property LossVariables As SDVariable()
			Set(ByVal lossVariables() As SDVariable)
				Dim varNames(lossVariables.Length - 1) As String
				For i As Integer = 0 To lossVariables.Length - 1
					varNames(i) = lossVariables(i).name()
				Next i
    
				setLossVariables(varNames)
			End Set
		End Property

		''' <summary>
		''' Mark the specified variable as a loss function variable. This means that this variable will be minimized via backprop during training.<br>
		''' This will add the variable as a loss to any others - i.e., if multiple variables are marked as losses, their values will be summed
		''' to give the total network loss.<br>
		''' Note that only floating point (Float16/32/64) variables may be marked as a loss.<br>
		''' Note also that only ARRAY type SDVariables can be marked as losses to be minimized. That is, we cannot mark the value
		''' of a constant, variable or placeholder to be minimized as doing so would not make sense.<br>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addLossVariable(@NonNull String variableName)
		Public Overridable Sub addLossVariable(ByVal variableName As String)
			Preconditions.checkState(hasVariable(variableName), "No variable with name ""%s"" exists", variableName)
			Dim v As SDVariable = getVariable(variableName)
			Preconditions.checkState(v.dataType().isFPType(), "Only floating point type variables can be marked as losses to be minimized." & " SDVariable ""%s"" has datatype %s", variableName, v.dataType())
			Preconditions.checkState(v.getVariableType() = VariableType.ARRAY, "Only ARRAY type SDVariables can be marked as losses to be minimized." & " SDVariable ""%s"" has variable type %s", variableName, v.getVariableType())
			If Not lossVariables_Conflict.Contains(variableName) Then
				lossVariables_Conflict.Add(variableName)
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="addLossVariable(String)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void addLossVariable(@NonNull SDVariable variable)
		Public Overridable Sub addLossVariable(ByVal variable As SDVariable)
			addLossVariable(variable.name())
		End Sub

		''' <summary>
		''' Set the training configuration (<seealso cref="TrainingConfig"/>) for the SameDiff instance.
		''' A TrainingConfig must be set before the SameDiff instance can be trained via the fit methods
		''' </summary>
		''' <param name="trainingConfig"> Training configuration </param>
'JAVA TO VB CONVERTER NOTE: The parameter trainingConfig was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable WriteOnly Property TrainingConfig As TrainingConfig
			Set(ByVal trainingConfig_Conflict As TrainingConfig)
				Me.trainingConfig_Conflict = trainingConfig_Conflict
			End Set
		End Property

		''' <summary>
		''' Fit the SameDiff instance based on a single DataSet (i.e., a single minibatch for one iteration).<br>
		''' This method can only be used for singe input, single output SameDiff instances as DataSet only supports a
		''' single input and a single output.<br>
		''' Note that a <seealso cref="TrainingConfig"/> must be set via <seealso cref="setTrainingConfig(TrainingConfig)"/> before training can
		''' be performed.
		''' </summary>
		''' <param name="dataSet">   The DataSet (single minibatch) to peform training on </param>
		''' <param name="listeners"> Additional listeners to use during this operation </param>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.listeners.records.History fit(@NonNull DataSet dataSet, @NonNull Listener... listeners)
		Public Overridable Function fit(ByVal dataSet As DataSet, ParamArray ByVal listeners() As Listener) As History
			Return fit(New SingletonMultiDataSetIterator(dataSet.toMultiDataSet()), 1, False, Nothing, 1, listeners)
		End Function

		''' <summary>
		''' Fit the SameDiff instance based on a single MultiDataSet (i.e., a single minibatch for one iteration).<br>
		''' Note that a <seealso cref="TrainingConfig"/> must be set via <seealso cref="setTrainingConfig(TrainingConfig)"/> before training can
		''' be performed.
		''' </summary>
		''' <param name="dataSet">   The MultiDataSet (single minibatch) to peform training on </param>
		''' <param name="listeners"> Additional listeners to use during this operation </param>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.listeners.records.History fit(@NonNull MultiDataSet dataSet, @NonNull Listener... listeners)
		Public Overridable Function fit(ByVal dataSet As MultiDataSet, ParamArray ByVal listeners() As Listener) As History
			Return fit(New SingletonMultiDataSetIterator(dataSet), 1, False, Nothing, 1, listeners)
		End Function

		''' <summary>
		''' Fit the SameDiff instance based on DataSetIterator for the specified number of epochs.<br>
		''' This method can only be used for singe input, single output SameDiff instances as DataSet only supports a
		''' single input and a single output.<br>
		''' Note that a <seealso cref="TrainingConfig"/> must be set via <seealso cref="setTrainingConfig(TrainingConfig)"/> before training can
		''' be performed.
		''' <para>
		''' A special case of <seealso cref="fit()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iter">                The iterator to train the SameDiff instance with </param>
		''' <param name="numEpochs">           The number of epochs for training. Must be > 0 </param>
		''' <param name="validationIter">      The DataSetIterator to use for validation (null to skip validation) </param>
		''' <param name="validationFrequency"> The frequency with which to run validation.  1 is every epoch, 2 is every other, etc. </param>
		''' <param name="listeners">           Additional listeners to use during this operation </param>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.listeners.records.History fit(@NonNull DataSetIterator iter, int numEpochs, org.nd4j.linalg.dataset.api.iterator.DataSetIterator validationIter, int validationFrequency, @NonNull Listener... listeners)
		Public Overridable Function fit(ByVal iter As DataSetIterator, ByVal numEpochs As Integer, ByVal validationIter As DataSetIterator, ByVal validationFrequency As Integer, ParamArray ByVal listeners() As Listener) As History
			Return fit().train(iter, numEpochs).validate(validationIter, validationFrequency).listeners(listeners).exec()
		End Function

		''' <summary>
		''' See <seealso cref="fit(DataSetIterator, Integer, DataSetIterator, Integer, Listener...)"/>, does not preform validation.
		''' <para>
		''' A special case of <seealso cref="fit()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iter">      The iterator to train the SameDiff instance with </param>
		''' <param name="numEpochs"> The number of epochs for training. Must be > 0 </param>
		''' <param name="listeners"> Additional listeners to use during this operation </param>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.listeners.records.History fit(@NonNull DataSetIterator iter, int numEpochs, @NonNull Listener... listeners)
		Public Overridable Function fit(ByVal iter As DataSetIterator, ByVal numEpochs As Integer, ParamArray ByVal listeners() As Listener) As History
			Return fit().train(iter, numEpochs).listeners(listeners).exec()
		End Function

		''' <summary>
		''' Fit the SameDiff instance based on MultiDataSetIterator for the specified number of epochs.<br>
		''' This method can both singe input, single output and multi-input, multi-output SameDiff instances<br>
		''' Note that a <seealso cref="TrainingConfig"/> must be set via <seealso cref="setTrainingConfig(TrainingConfig)"/> before training can
		''' be performed.
		''' <para>
		''' A special case of <seealso cref="fit()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iter">                The iterator to train the SameDiff instance with </param>
		''' <param name="numEpochs">           The number of epochs for training. Must be > 0 </param>
		''' <param name="validationIter">      The MultiDataSetIterator to use for validation (null to skip validation) </param>
		''' <param name="validationFrequency"> The frequency with which to run validation.  1 is every epoch, 2 is every other, etc. </param>
		''' <param name="listeners">           Additional listeners to use during this operation </param>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.listeners.records.History fit(@NonNull MultiDataSetIterator iter, int numEpochs, org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator validationIter, int validationFrequency, @NonNull Listener... listeners)
		Public Overridable Function fit(ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer, ByVal validationIter As MultiDataSetIterator, ByVal validationFrequency As Integer, ParamArray ByVal listeners() As Listener) As History
			Return fit(iter, numEpochs, True, validationIter, validationFrequency, listeners)
		End Function

		''' <summary>
		''' See <seealso cref="fit(MultiDataSetIterator, Integer, MultiDataSetIterator, Integer, Listener...)"/>, does not preform validation.
		''' <para>
		''' A special case of <seealso cref="fit()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iter">      The iterator to train the SameDiff instance with </param>
		''' <param name="numEpochs"> The number of epochs for training. Must be > 0 </param>
		''' <param name="listeners"> Additional listeners to use during this operation </param>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public org.nd4j.autodiff.listeners.records.History fit(@NonNull MultiDataSetIterator iter, int numEpochs, @NonNull Listener... listeners)
		Public Overridable Function fit(ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer, ParamArray ByVal listeners() As Listener) As History
			Return fit().train(iter, numEpochs).listeners(listeners).exec()
		End Function

		''' <summary>
		''' Set up for a fit operation using <seealso cref="FitConfig"/>.
		''' <para>
		''' Supports the setting of training data (<seealso cref="MultiDataSetIterator"/> or <seealso cref="DataSetIterator"/>), number of epochs,
		''' validation data (<seealso cref="MultiDataSetIterator"/> or <seealso cref="DataSetIterator"/>), validation frequency, and additional listeners.
		''' <br><br>
		''' Example: train on data for 5 epochs, validating on valData every 2nd epoch
		''' <pre>
		'''     {@code
		'''     SameDiff sd = ...;
		'''     MultiDataSet data = ...;
		'''     MultiDataSet valData = ...;
		''' 
		'''     History hist = sd.fit()
		'''         .train(data, 5)
		'''         .validate(valData, 2)
		'''         .exec();
		'''     }
		''' </pre>
		''' </para>
		''' </summary>
		Public Overridable Function fit() As FitConfig
			Return New FitConfig(Me)
		End Function

		'Synchronized for thread safety
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected synchronized org.nd4j.autodiff.listeners.records.History fit(@NonNull MultiDataSetIterator iter, int numEpochs, boolean incrementEpochCount, org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator validationData, int validationFrequency, @NonNull Listener... listeners)
		Protected Friend Overridable Function fit(ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer, ByVal incrementEpochCount As Boolean, ByVal validationData As MultiDataSetIterator, ByVal validationFrequency As Integer, ParamArray ByVal listeners() As Listener) As History
			SyncLock Me
				Dim async As Boolean = iter.asyncSupported()
        
				Dim validationAsync As Boolean = False
				If validationData IsNot Nothing Then
					validationAsync = validationData.asyncSupported()
				End If
        
				If async Then
					iter = New AsyncMultiDataSetIterator(iter, 3, True)
				End If
        
				If validationAsync Then
					validationData = New AsyncMultiDataSetIterator(validationData, 3, True)
				End If
        
				Try
					Return fitHelper(iter, numEpochs, incrementEpochCount, validationData, validationFrequency, New List(Of Listener) From {listeners})
				Finally
					If async Then
						CType(iter, AsyncMultiDataSetIterator).shutdown()
					End If
					If validationAsync Then
						DirectCast(validationData, AsyncMultiDataSetIterator).shutdown()
					End If
				End Try
			End SyncLock
		End Function

		'fitHelper should only be called from fit method above
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected synchronized org.nd4j.autodiff.listeners.records.History fitHelper(@NonNull MultiDataSetIterator iter, int numEpochs, boolean incrementEpochCount, org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator validationData, int validationFrequency, @NonNull List<Listener> listeners)
		Protected Friend Overridable Function fitHelper(ByVal iter As MultiDataSetIterator, ByVal numEpochs As Integer, ByVal incrementEpochCount As Boolean, ByVal validationData As MultiDataSetIterator, ByVal validationFrequency As Integer, ByVal listeners As IList(Of Listener)) As History
			SyncLock Me
				Preconditions.checkNotNull(iter, "Iterator must not be null")
				Preconditions.checkState(numEpochs > 0, "Number of training epochs must be a positive number. Got: %s", numEpochs)
				Preconditions.checkState(trainingConfig_Conflict IsNot Nothing, "No training configuration has been set. A training configuration must " & "be set before training. Use setTrainingConfig(TrainingConfig)")
				Preconditions.checkState(numEpochs = 1 OrElse iter.resetSupported(), "Cannot train for multiple epochs on an iterator that" & " does not support resetting")
        
				Dim history As New HistoryListener(trainingConfig_Conflict)
        
				Dim activeListeners As IList(Of Listener) = New List(Of Listener)()
        
				activeListeners.Add(history)
        
				For Each l As Listener In Me.listeners_Conflict
					If l.isActive(Operation.TRAINING) Then
						activeListeners.Add(l)
					End If
				Next l
        
				For Each l As Listener In listeners
					If l.isActive(Operation.TRAINING) Then
						activeListeners.Add(l)
					End If
				Next l
        
				validateListenerActivations(activeListeners, Operation.TRAINING)
				validateListenerActivations(activeListeners, Operation.TRAINING_VALIDATION)
        
				If Not iter.hasNext() AndAlso iter.resetSupported() Then
					iter.reset()
				End If
        
				Dim performedValidation As Boolean = False
        
				Dim trainThreadNum As Integer = 0
				Dim jThreadId As Long = Thread.CurrentThread.getId()
				Dim hasListeners As Boolean = activeListeners.Count > 0
'JAVA TO VB CONVERTER NOTE: The variable at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim at_Conflict As At = At.builder().epoch(trainingConfig_Conflict.getEpochCount()).iteration(trainingConfig_Conflict.getIterationCount()).trainingThreadNum(trainThreadNum).javaThreadNum(jThreadId).operation(Operation.TRAINING).build()
        
				Dim lossCurve As LossCurve = Nothing
        
        
				Dim requiredVars As ISet(Of String) = New HashSet(Of String)()
				For Each l As Listener In activeListeners
					Dim lv As ListenerVariables = l.requiredVariables(Me)
					If lv IsNot Nothing Then
						Dim s As ISet(Of String) = lv.trainingVariables()
						If s IsNot Nothing Then
							requiredVars.addAll(s)
						End If
					End If
				Next l
        
				Dim listenersWitHistory As IList(Of Listener) = New List(Of Listener)(listeners)
				For Each l As Listener In Me.listeners_Conflict
					If Not listenersWitHistory.Contains(l) Then
						listenersWitHistory.Add(l)
					End If
				Next l
				listenersWitHistory.Add(history)
        
        
				Dim gradInstance As SameDiff = getFunction("grad")
				If gradInstance Is Nothing Then
					createGradFunction()
					gradInstance = getFunction("grad")
				End If
				Dim ts As New TrainingSession(gradInstance)
				gradInstance.TrainingConfig = trainingConfig_Conflict 'In case any listeners want to use it
        
				For Each l As Listener In activeListeners
					l.operationStart(gradInstance, Operation.TRAINING)
				Next l
        
				Dim paramsToTrain As ISet(Of String) = New LinkedHashSet(Of String)()
				For Each v As Variable In variables_Conflict.Values
					If v.getVariable().getVariableType() = VariableType.VARIABLE Then
						'TODO not all variable type are needed - i.e., variable that doesn't impact loss should be skipped
						paramsToTrain.Add(v.getName())
					End If
				Next v
        
				Dim lastLoss As Loss = Nothing
				For i As Integer = 0 To numEpochs - 1
					If incrementEpochCount AndAlso hasListeners Then
						at_Conflict.setEpoch(trainingConfig_Conflict.getEpochCount())
						For Each l As Listener In activeListeners
							l.epochStart(Me, at_Conflict)
						Next l
					End If
					Dim epochStartTime As Long = DateTimeHelper.CurrentUnixTimeMillis()
        
					Dim lossSums() As Double = Nothing
					Dim lossNames As IList(Of String) = Nothing
					Dim lossCount As Integer = 0
        
					Do While iter.hasNext()
						Dim dataStart As Long = If(hasListeners, DateTimeHelper.CurrentUnixTimeMillis(), 0)
						Dim ds As MultiDataSet = iter.next()
        
						Dim dataEnd As Long = If(hasListeners, DateTimeHelper.CurrentUnixTimeMillis(), 0)
						If Not performedValidation Then
							Preconditions.checkState(trainingConfig_Conflict.getDataSetFeatureMapping().size() = ds.numFeatureArrays(), "The number of dataset feature mapping variables set in the training configuration (%s) must match" & " the number of dataset feature arrays (%s)", trainingConfig_Conflict.getDataSetFeatureMapping().size(), ds.numFeatureArrays())
							Dim labelMapping As IList(Of String) = trainingConfig_Conflict.getDataSetLabelMapping()
							Dim lblSize As Integer = If(labelMapping Is Nothing, 0, labelMapping.Count)
							Preconditions.checkState(lblSize = ds.numLabelsArrays(), "The number of dataset label mapping variables set in the training configuration (%s) must match" & " the number of dataset label arrays (%s)", lblSize, ds.numLabelsArrays())
        
							performedValidation = True
						End If
        
						If hasListeners Then
							at_Conflict.setIteration(trainingConfig_Conflict.getIterationCount())
							For Each l As Listener In activeListeners
								l.iterationStart(Me, at_Conflict, ds, (dataEnd - dataStart))
							Next l
						End If
        
						'Create placeholder variable map
						Dim placeholders As IDictionary(Of String, INDArray) = toPlaceholderMap(ds)
        
						Preconditions.checkState(placeholders.Count > 0, "No placeholder variables were set for training")
        
						'Call TrainingSession to perform training
						If Not initializedTraining Then
							initializeTraining()
						End If
        
						lastLoss = ts.trainingIteration(trainingConfig_Conflict, placeholders, paramsToTrain, updaterMap, ds, getLossVariables(), listenersWitHistory, at_Conflict)
        
        
						If lossSums Is Nothing Then
							lossSums = lastLoss.getLosses().clone()
						Else
							For j As Integer = 0 To lossSums.Length - 1
								lossSums(j) += lastLoss.getLosses()(j)
							Next j
						End If
						lossCount += 1
        
						trainingConfig_Conflict.incrementIterationCount()
					Loop
        
					Dim epochTime As Long = DateTimeHelper.CurrentUnixTimeMillis() - epochStartTime
        
					If incrementEpochCount Then
						lossNames = lastLoss.getLossNames()
        
						For j As Integer = 0 To lossSums.Length - 1
							lossSums(j) /= lossCount
						Next j
        
						If lossCurve IsNot Nothing Then
							lossCurve = lossCurve.addLossAndCopy(lossSums, lossNames)
						Else
							lossCurve = New LossCurve(lossSums, lossNames)
						End If
					End If
        
        
					If incrementEpochCount Then
						If hasListeners Then
							Dim doStop As Boolean = False
							Dim stopped As Listener = Nothing
        
							For Each l As Listener In activeListeners
								Dim res As ListenerResponse = l.epochEnd(Me, at_Conflict, lossCurve, epochTime)
        
								If res = ListenerResponse.STOP AndAlso (i < numEpochs - 1) Then
									doStop = True
									stopped = l
								End If
							Next l
        
							If doStop Then
								log.info("Stopping training early.  Listener " & stopped & " gave a STOP signal at epoch " & at_Conflict.epoch() & " and iteration " & at_Conflict.iteration())
        
								For Each l1 As Listener In activeListeners
									l1.operationEnd(Me, Operation.TRAINING)
								Next l1
        
								If i < numEpochs - 1 Then
									iter.reset()
								End If
        
								If incrementEpochCount Then
									trainingConfig_Conflict.incrementEpochCount()
								End If
								Return history.Report
							End If
        
        
							'validation evaluation
							If validationData IsNot Nothing AndAlso (validationFrequency <= 0 OrElse i Mod validationFrequency = 0) Then
        
								Dim validationStart As Long = DateTimeHelper.CurrentUnixTimeMillis()
								outputHelper(validationData, New At(at_Conflict.epoch(), 0, 0, 0, Nothing, Operation.TRAINING_VALIDATION), listenersWitHistory)
        
								Dim validationTime As Long = DateTimeHelper.CurrentUnixTimeMillis() - validationStart
        
								Dim doStopV As Boolean = False
								Dim stoppedV As Listener = Nothing
								For Each l As Listener In activeListeners
        
									Dim res As ListenerResponse = l.validationDone(Me, at_Conflict, validationTime)
        
									If res = ListenerResponse.STOP AndAlso (i < numEpochs - 1) Then
										doStopV = True
										stoppedV = l
									End If
								Next l
        
								If doStopV Then
									log.info("Stopping training early from validation.  Listener " & stoppedV & " gave a STOP signal at epoch " & at_Conflict.epoch() & " and iteration " & at_Conflict.iteration())
        
									For Each l1 As Listener In activeListeners
										l1.operationEnd(Me, Operation.TRAINING)
									Next l1
        
									If i < numEpochs - 1 Then
										iter.reset()
									End If
        
									If incrementEpochCount Then
										trainingConfig_Conflict.incrementEpochCount()
									End If
        
									Return history.Report
								End If
        
							End If
        
						End If
        
						trainingConfig_Conflict.incrementEpochCount()
					End If
					If i < numEpochs - 1 Then
						iter.reset()
					End If
				Next i
        
				For Each l1 As Listener In activeListeners
					l1.operationEnd(Me, Operation.TRAINING)
				Next l1
        
				Return history.Report
			End SyncLock
		End Function

		''' <summary>
		''' Ensure the specified listeners do not request any activations that aren't present for the given operation
		''' </summary>
		Private Sub validateListenerActivations(ByVal listeners As IList(Of Listener), ByVal op As Operation)
			For Each l As Listener In listeners
				Dim lv As ListenerVariables = l.requiredVariables(Me)
				If lv IsNot Nothing Then
					For Each s As String In lv.requiredVariables(op)
						If Not variables_Conflict.ContainsKey(s) Then
							Preconditions.checkState(False, "Listener %s requested variable %s that is not defined in this SameDiff graph", l, s)
						End If
					Next s
				End If
			Next l
		End Sub

		''' <summary>
		''' Calculate the regularization (L1, L2 and/or WeightDecay) component of the loss function for the current parameters..
		''' Note that the training configuration must be set (via <seealso cref="setTrainingConfig(TrainingConfig)"/>) before this
		''' method can be called
		''' </summary>
		''' <returns> The regularization component of the score/loss function </returns>
		Public Overridable Function calcRegularizationScore() As Double
			Preconditions.checkState(trainingConfig_Conflict IsNot Nothing, "No training configuration has been set. A training configuration must " & "be set before calculating the L2 loss. Use setTrainingConfig(TrainingConfig)")

			If trainingConfig_Conflict.getRegularization() Is Nothing OrElse trainingConfig_Conflict.getRegularization().isEmpty() Then
				Return 0.0
			End If

			Dim l As IList(Of Regularization) = trainingConfig_Conflict.getRegularization()
'JAVA TO VB CONVERTER NOTE: The variable loss was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim loss_Conflict As Double = 0.0
			For Each v As Variable In variables_Conflict.Values
				Dim sdv As SDVariable = v.getVariable()
				If sdv.getVariableType() <> VariableType.VARIABLE OrElse Not sdv.dataType().isFPType() Then
					'Only trainable parameters (FP and variable type vars) contribute to regularization score
					Continue For
				End If
				For Each r As Regularization In l
					Dim arr As INDArray = sdv.Arr
					loss_Conflict += r.score(arr, trainingConfig_Conflict.getIterationCount(), trainingConfig_Conflict.getEpochCount())
				Next r
			Next v
			Return loss_Conflict
		End Function

		''' <summary>
		''' Perform setup for training. Does the following:
		''' 1. Infer the set of trainable parameters - unless specified manually by the user
		''' 2. Set up the updaters
		''' </summary>
		Protected Friend Overridable Sub initializeTraining()
			If Not initializedTraining Then
				If trainingConfig_Conflict Is Nothing Then
					Throw New ND4JIllegalStateException("Please specify a training config with setTrainingConfig")
				End If
				updaterMap = New Dictionary(Of String, GradientUpdater)()
				For Each v As Variable In variables_Conflict.Values
					If v.getVariable().getVariableType() <> VariableType.VARIABLE OrElse Not v.getVariable().dataType().isFPType() Then
						'Skip non-trainable parameters
						Continue For
					End If

					Dim arr As INDArray = v.getVariable().getArr()
					Dim stateSize As Long = trainingConfig_Conflict.getUpdater().stateSize(arr.length())
					Dim view As INDArray = If(stateSize = 0, Nothing, Nd4j.createUninitialized(arr.dataType(), 1, stateSize))
					Dim gu As GradientUpdater = trainingConfig_Conflict.getUpdater().instantiate(view, False)
					gu.setStateViewArray(view, arr.shape(), arr.ordering(), True)
					updaterMap(v.getName()) = gu
				Next v

				initializedTraining = True
			End If
		End Sub

		''' <summary>
		''' Convert the MultiDataSet to a {@code Map<String,INDArray>} based on the TrainingConfig settings.
		''' The key is the placeholder/variable that the value INDArray should be associated with.
		''' </summary>
		''' <param name="ds"> MultiDataSet - source of the features/labels </param>
		''' <returns> MultiDataSet converted to a Map, based on TrainingConfig </returns>
		Private Function toPlaceholderMap(ByVal ds As MultiDataSet) As IDictionary(Of String, INDArray)
			Dim placeholders As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim count As Integer = 0
			For Each s As String In trainingConfig_Conflict.getDataSetFeatureMapping()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: placeholders.put(s, ds.getFeatures(count++));
				placeholders(s) = ds.getFeatures(count)
					count += 1
			Next s
			count = 0
			If trainingConfig_Conflict.getDataSetLabelMapping() IsNot Nothing Then
				'Labels may be null in some models (unsupervised etc)
				For Each s As String In trainingConfig_Conflict.getDataSetLabelMapping()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: placeholders.put(s, ds.getLabels(count++));
					placeholders(s) = ds.getLabels(count)
						count += 1
				Next s
			End If

			If trainingConfig_Conflict.getDataSetFeatureMaskMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetFeatureMaskMapping().size() > 0 Then
				count = 0
				For Each s As String In trainingConfig_Conflict.getDataSetFeatureMaskMapping()
					If s Is Nothing Then
						count += 1
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: placeholders.put(s, ds.getFeaturesMaskArray(count++));
					placeholders(s) = ds.getFeaturesMaskArray(count)
						count += 1
				Next s
			End If

			If trainingConfig_Conflict.getDataSetLabelMaskMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetLabelMaskMapping().size() > 0 Then
				count = 0
				For Each s As String In trainingConfig_Conflict.getDataSetLabelMaskMapping()
					If s Is Nothing Then
						count += 1
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: placeholders.put(s, ds.getLabelsMaskArray(count++));
					placeholders(s) = ds.getLabelsMaskArray(count)
						count += 1
				Next s
			End If
			Return placeholders
		End Function

		''' <summary>
		''' Evaluate the performance of a single variable's prediction.<br>
		''' For example, if the variable to evaluatate was called "softmax" you would use:
		''' <pre>
		''' {@code Evaluation e = new Evaluation();
		''' sameDiff.evaluate(iterator, "softmax", e);}
		''' </pre>
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iterator">       Iterator as source of data to evaluate </param>
		''' <param name="outputVariable"> The variable to evaluate </param>
		''' <param name="listeners">      Additional listeners to use during this operation. </param>
		''' <param name="evaluations">    The evaluations to perform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void evaluate(@NonNull DataSetIterator iterator, @NonNull String outputVariable, @NonNull List<Listener> listeners, @NonNull IEvaluation... evaluations)
		Public Overridable Sub evaluate(ByVal iterator As DataSetIterator, ByVal outputVariable As String, ByVal listeners As IList(Of Listener), ParamArray ByVal evaluations() As IEvaluation)
			Preconditions.checkArgument(evaluations IsNot Nothing AndAlso evaluations.Length > 0, "No evaluations were passed to the evaluate method")

			evaluate().data(iterator).evaluate(outputVariable, evaluations).listeners(CType(listeners, List(Of Listener)).ToArray()).exec()
		End Sub

		''' <summary>
		''' See <seealso cref="evaluate(DataSetIterator, String, List, IEvaluation[])"/>.
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void evaluate(@NonNull DataSetIterator iterator, @NonNull String outputVariable, @NonNull IEvaluation... evaluations)
		Public Overridable Sub evaluate(ByVal iterator As DataSetIterator, ByVal outputVariable As String, ParamArray ByVal evaluations() As IEvaluation)
			evaluate().data(iterator).evaluate(outputVariable, evaluations).exec()
		End Sub

		''' <summary>
		''' Evaluation for multiple-output networks.<br>
		''' See <seealso cref="evaluate(MultiDataSetIterator, Map, Map, Listener[])"/>.
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void evaluate(@NonNull DataSetIterator iterator, @NonNull Map<String, org.nd4j.evaluation.IEvaluation> variableEvals, @NonNull Listener... listeners)
		Public Overridable Sub evaluate(ByVal iterator As DataSetIterator, ByVal variableEvals As IDictionary(Of String, IEvaluation), ParamArray ByVal listeners() As Listener)
			Dim map As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			Dim variableEvalsList As IDictionary(Of String, IList(Of IEvaluation)) = New Dictionary(Of String, IList(Of IEvaluation))()
			For Each s As String In variableEvals.Keys
				map(s) = 0 'Only 1 possible output here with DataSetIterator
				variableEvalsList(s) = Collections.singletonList(variableEvals(s))
			Next s
			evaluate(New MultiDataSetIteratorAdapter(iterator), variableEvalsList, map, listeners)
		End Sub

		''' <summary>
		''' Evaluation for multiple output networks - one or more.
		''' See <seealso cref="evaluate(MultiDataSetIterator, Map, Map, Listener[])"/>.
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void evaluateMultiple(org.nd4j.linalg.dataset.api.iterator.DataSetIterator iterator, Map<String, List<org.nd4j.evaluation.IEvaluation>> variableEvals, @NonNull Listener... listeners)
		Public Overridable Sub evaluateMultiple(ByVal iterator As DataSetIterator, ByVal variableEvals As IDictionary(Of String, IList(Of IEvaluation)), ParamArray ByVal listeners() As Listener)
			Dim map As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
			For Each s As String In variableEvals.Keys
				map(s) = 0 'Only 1 possible output here with DataSetIterator
			Next s
			evaluate(New MultiDataSetIteratorAdapter(iterator), variableEvals, map, listeners)
		End Sub

		''' <summary>
		''' Evaluate the performance of a single variable's prediction.<br>
		''' For example, if the variable to evaluatate was called "softmax" you would use:
		''' <pre>
		''' {@code Evaluation e = new Evaluation();
		''' sameDiff.evaluate(iterator, "softmax", e);}
		''' </pre>
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iterator">       Iterator as source of data to evaluate </param>
		''' <param name="outputVariable"> The variable to evaluate </param>
		''' <param name="labelIndex">     The index of the target variable's labels in the iterator </param>
		''' <param name="listeners">      Additional listeners to use during this operation. </param>
		''' <param name="evaluations">    The evaluations to perform </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void evaluate(@NonNull MultiDataSetIterator iterator, @NonNull String outputVariable, int labelIndex, @NonNull List<Listener> listeners, @NonNull IEvaluation... evaluations)
		Public Overridable Sub evaluate(ByVal iterator As MultiDataSetIterator, ByVal outputVariable As String, ByVal labelIndex As Integer, ByVal listeners As IList(Of Listener), ParamArray ByVal evaluations() As IEvaluation)
			Preconditions.checkArgument(evaluations IsNot Nothing AndAlso evaluations.Length > 0, "No evaluations were passed to the evaluate method")

			evaluate().data(iterator).evaluate(outputVariable, labelIndex, evaluations).listeners(CType(listeners, List(Of Listener)).ToArray()).exec()
		End Sub

		''' <summary>
		''' See <seealso cref="evaluate(MultiDataSetIterator, String, Integer, List, IEvaluation[])"/>.
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void evaluate(@NonNull MultiDataSetIterator iterator, @NonNull String outputVariable, int labelIndex, @NonNull IEvaluation... evaluations)
		Public Overridable Sub evaluate(ByVal iterator As MultiDataSetIterator, ByVal outputVariable As String, ByVal labelIndex As Integer, ParamArray ByVal evaluations() As IEvaluation)
			evaluate().data(iterator).evaluate(outputVariable, labelIndex, evaluations).exec()
		End Sub

		''' <summary>
		''' Perform evaluation using classes such as <seealso cref="Evaluation"/> for classifier outputs
		''' and <seealso cref="org.nd4j.evaluation.regression.RegressionEvaluation"/> for regression outputs.<br>
		''' <br>
		''' <b>Example: classifier evaluation</b><br>
		''' Predictions variable name: "softmaxOutput"<br>
		''' Evaluations to perform: <seealso cref="Evaluation"/><br>
		''' Data: single input, single output MultiDataSets<br>
		''' Code:<br>
		''' <pre>
		''' {@code
		''' MultiDataSetIterator data = ...
		''' Map<String,List<IEvaluation>> evals = Collections.singletonMap("softmaxOutput",Collections.singletonList(new Evaluation()));
		''' Map<String,Integer> labelMapping = Collections.singletonMap("softmaxOutput",0);  //Compare: "softmaxOutput" vs. MultiDataSet.getLabels(0)
		''' }
		''' </pre>
		''' <para>
		''' A special case of <seealso cref="evaluate()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iterator">               The iterator - the source of the data for evaluation </param>
		''' <param name="variableEvals">          The evaluations to perform. Key: the name of the variable. Value: the evaluations to perform </param>
		''' <param name="predictionLabelMapping"> The output/label mapping. Key: the name of the variable. </param>
		''' <param name="listeners">              Additional listeners to use during this operation. </param>
		Public Overridable Sub evaluate(ByVal iterator As MultiDataSetIterator, ByVal variableEvals As IDictionary(Of String, IList(Of IEvaluation)), ByVal predictionLabelMapping As IDictionary(Of String, Integer), ParamArray ByVal listeners() As Listener)
			evaluateHelper(iterator, variableEvals, predictionLabelMapping, At.defaultAt(Operation.EVALUATION), listeners)
		End Sub


		''' <summary>
		''' Set up for a evaluation operation using EvaluationConfig.
		''' <para>
		''' Supports the setting of the data (<seealso cref="MultiDataSetIterator"/> or <seealso cref="DataSetIterator"/>),
		''' adding evaluations for variables (with optional label index setting), setting label indices,
		''' and setting additional listeners.
		''' Does not require setting label indices when using a <seealso cref="DataSetIterator"/>.
		''' </para>
		''' <para>
		''' Also supports using <seealso cref="SDVariable"/> instances instead of variable names.
		''' 
		''' <br><br>
		''' Example: evaluate "pred" with <seealso cref="Evaluation"/> and <seealso cref="ROC"/>, using label 0.
		''' <pre>
		'''      {@code
		'''     SameDiff sd = ...;
		'''     MultiDataSetIterator data = ...;
		''' 
		'''     EvaluationRecord results = sd.evaluate()
		'''         .data(data)
		'''         .evaluate("pred", 0, new Evaluation(), new ROC()),
		'''         .exec();
		'''      }
		'''  </pre>
		''' Example: evaluate "pred" with <seealso cref="Evaluation"/>, using the only label from a DataSetIterator.
		''' <pre>
		'''      {@code
		'''     SameDiff sd = ...;
		'''     DataSetIterator singleData = ...;
		''' 
		'''     EvaluationRecord results = sd.evaluate()
		'''         .data(singleData)
		'''         .evaluate("pred", new Evaluation()),
		'''         .exec();
		'''      }
		'''  </pre>
		''' </para>
		''' </summary>
		Public Overridable Function evaluate() As EvaluationConfig
			Return New EvaluationConfig(Me)
		End Function

		''' <summary>
		''' Helper method for evaluations.  Should only be called from the above evaluate method
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private void evaluateHelper(org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator iterator, Map<String, List<org.nd4j.evaluation.IEvaluation>> variableEvals, Map<String, Integer> predictionLabelMapping, At at, @NonNull Listener... listeners)
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Sub evaluateHelper(ByVal iterator As MultiDataSetIterator, ByVal variableEvals As IDictionary(Of String, IList(Of IEvaluation)), ByVal predictionLabelMapping As IDictionary(Of String, Integer), ByVal at_Conflict As At, ParamArray ByVal listeners() As Listener)
			Preconditions.checkState(trainingConfig_Conflict IsNot Nothing, "Training config has not been set")

			Preconditions.checkState(variableEvals.Keys.Equals(predictionLabelMapping.Keys), "Keysets for variable evaluations" & " and for the prediction label mapping must be equal. Keys for variables to evaluate: %s vs. keys for label mapping: %s", variableEvals.Keys, predictionLabelMapping.Keys)

			Dim activeListeners As IList(Of Listener) = New List(Of Listener)()

			For Each l As Listener In listeners
				If l.isActive(at_Conflict.operation()) Then
					activeListeners.Add(l)
				End If
			Next l

			For Each l As Listener In Me.listeners_Conflict
				If l.isActive(at_Conflict.operation()) Then
					activeListeners.Add(l)
				End If
			Next l

			validateListenerActivations(activeListeners, at_Conflict.operation())

			For Each l As Listener In activeListeners
				l.operationStart(Me, at_Conflict.operation())
			Next l

			Dim hasListeners As Boolean = activeListeners.Count > 0

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() AndAlso iterator.resetSupported() Then
				iterator.reset()
			End If
			Dim requiredVars As ISet(Of String) = New HashSet(Of String)(variableEvals.Keys)

			If hasListeners Then
				For Each l As Listener In activeListeners
					Dim v As ListenerVariables = l.requiredVariables(Me)
					If v IsNot Nothing Then
						requiredVars.addAll(v.evaluationVariables())
					End If
				Next l
			End If

			Dim requiredVarsArr() As String = requiredVars.ToArray()

			Do While iterator.MoveNext()
				Dim ds As MultiDataSet = iterator.Current
				Dim placeholderMap As IDictionary(Of String, INDArray) = toPlaceholderMap(ds)

				Dim m As IDictionary(Of String, INDArray) = directExecHelper(placeholderMap, at_Conflict, ds, Enumerable.Empty(Of String)(), activeListeners, requiredVarsArr)

				For Each e As KeyValuePair(Of String, IList(Of IEvaluation)) In variableEvals.SetOfKeyValuePairs()
					Dim prediction As INDArray = m(e.Key)
					For Each eval As IEvaluation In e.Value
						'TODO time series, etc

						Dim label As INDArray = ds.getLabels(predictionLabelMapping(e.Key))
						Dim mask As INDArray = ds.getLabelsMaskArray(predictionLabelMapping(e.Key))
						eval.eval(label, prediction, mask)
					Next eval
				Next e

				at_Conflict.setIteration(at_Conflict.iteration() + 1)
			Loop


			For Each l As Listener In activeListeners
				l.operationEnd(Me, at_Conflict.operation())
			Next l
		End Sub

		''' <summary>
		''' Do a single batch inference on a network with a single input.<br>
		''' For example, if the variable to infer was called "softmax" you would use:
		''' <pre>
		''' {@code
		''' sameDiff.output(iterator, "softmax");}
		''' </pre>
		''' </summary>
		''' <param name="dataSet"> The data to evaluate </param>
		''' <param name="outputs"> The variables to evaluate </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(@NonNull DataSet dataSet, @NonNull String... outputs)
		Public Overridable Function output(ByVal dataSet As DataSet, ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return outputBatches(New SingletonMultiDataSetIterator(dataSet.toMultiDataSet()), outputs)(0)
		End Function

		''' <summary>
		''' Do a single batch inference on a network.<br>
		''' For example, if the variable to infer was called "softmax" you would use:
		''' <pre>
		''' {@code
		''' sameDiff.output(iterator, "softmax");}
		''' </pre>
		''' </summary>
		''' <param name="dataSet"> The data to evaluate </param>
		''' <param name="outputs"> The variables to evaluate </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(@NonNull MultiDataSet dataSet, @NonNull String... outputs)
		Public Overridable Function output(ByVal dataSet As MultiDataSet, ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return outputBatches(New SingletonMultiDataSetIterator(dataSet), outputs)(0)
		End Function

		''' <summary>
		''' Do inference on a network with a single input.<br>
		''' For example, if the variable to infer was called "softmax" you would use:
		''' <pre>
		''' {@code
		''' sameDiff.output(iterator, "softmax");}
		''' </pre>
		''' <para>
		''' Uses concatenation on the outputs of <seealso cref="outputBatches(DataSetIterator, String...)"/> which may cause issues with some inputs.
		''' RNNs with variable time series length and CNNs with variable image sizes will most likely have issues.
		''' </para>
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iterator">  Iterator as source of data to evaluate </param>
		''' <param name="listeners"> Additional listeners to use during this operation. </param>
		''' <param name="outputs">   The variables to evaluate </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(@NonNull DataSetIterator iterator, @NonNull List<Listener> listeners, @NonNull String... outputs)
		Public Overridable Function output(ByVal iterator As DataSetIterator, ByVal listeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return output().data(iterator).output(outputs).listeners(CType(listeners, List(Of Listener)).ToArray()).exec()
		End Function

		''' <summary>
		''' See <seealso cref="output(DataSetIterator, List, String...)"/>.  No additional listeners.
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(@NonNull DataSetIterator dataSet, @NonNull String... outputs)
		Public Overridable Function output(ByVal dataSet As DataSetIterator, ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return output().data(dataSet).output(outputs).exec()
		End Function


		''' <summary>
		''' See <seealso cref="output(DataSetIterator, List, String...)"/>, but without the concatenation of batches.
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' </para>
		''' </summary>
		Public Overridable Function outputBatches(ByVal iterator As DataSetIterator, ByVal listeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IList(Of IDictionary(Of String, INDArray))
			Return output().data(iterator).output(outputs).listeners(CType(listeners, List(Of Listener)).ToArray()).execBatches()
		End Function


		''' <summary>
		''' See <seealso cref="output(DataSetIterator, String...)"/>, but without the concatenation of batches.
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' </para>
		''' </summary>
		Public Overridable Function outputBatches(ByVal iterator As DataSetIterator, ParamArray ByVal outputs() As String) As IList(Of IDictionary(Of String, INDArray))
			Return output().data(iterator).output(outputs).execBatches()
		End Function

		''' <summary>
		''' Perform inference.<br>
		''' <br>
		''' <b>Example: classifier inference</b><br>
		''' Predictions variable name: "softmaxOutput"<br>
		''' Evaluations to perform: <seealso cref="Evaluation"/><br>
		''' Data: single output MultiDataSets<br>
		''' Code:<br>
		''' <pre>
		''' {@code
		''' MultiDataSetIterator data = ...
		''' sameDiff.output(iterator, "softmaxOutput);
		''' }
		''' </pre>
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iterator">  The iterator - the source of the data for inference </param>
		''' <param name="listeners"> Additional listeners to use during this operation. </param>
		''' <param name="outputs">   The set of outputs to report.  If null, defaults to all outputs of this SameDiff. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(@NonNull MultiDataSetIterator iterator, @NonNull List<Listener> listeners, @NonNull String... outputs)
		Public Overridable Function output(ByVal iterator As MultiDataSetIterator, ByVal listeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return stackOutputs(outputHelper(iterator, At.defaultAt(Operation.INFERENCE), listeners, outputs))
		End Function

		''' <summary>
		''' See <seealso cref="output(MultiDataSetIterator, List, String...)"/>.  No additional listeners.
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(@NonNull MultiDataSetIterator dataSet, @NonNull String... outputs)
		Public Overridable Function output(ByVal dataSet As MultiDataSetIterator, ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return output().data(dataSet).output(outputs).exec()
		End Function

		''' <summary>
		''' Perform inference.<br>
		''' <br>
		''' <b>Example: classifier inference</b><br>
		''' Predictions variable name: "softmaxOutput"<br>
		''' Evaluations to perform: <seealso cref="Evaluation"/><br>
		''' Data: single output MultiDataSets<br>
		''' Code:<br>
		''' <pre>
		''' {@code
		''' MultiDataSetIterator data = ...
		''' sameDiff.output(iterator, "softmaxOutput);
		''' }
		''' </pre>
		''' <para>
		''' Uses concatenation on the outputs of <seealso cref="outputBatches(MultiDataSetIterator, List, String...)"/> which may cause issues with some inputs.
		''' RNNs with variable time series length and CNNs with variable image sizes will most likely have issues.
		''' </para>
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="iterator">  The iterator - the source of the data for inference </param>
		''' <param name="listeners"> Additional listeners to use during this operation. </param>
		''' <param name="outputs">   The set of outputs to report.  If null, defaults to all outputs of this SameDiff. </param>
		Public Overridable Function outputBatches(ByVal iterator As MultiDataSetIterator, ByVal listeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IList(Of IDictionary(Of String, INDArray))
			Return outputHelper(iterator, At.defaultAt(Operation.INFERENCE), listeners, outputs)
		End Function

		''' <summary>
		''' See <seealso cref="outputBatches(MultiDataSetIterator, List, String...)"/>.  No additional listeners.
		''' <para>
		''' Special case of <seealso cref="output()"/>.
		''' </para>
		''' </summary>
		Public Overridable Function outputBatches(ByVal iterator As MultiDataSetIterator, ParamArray ByVal outputs() As String) As IList(Of IDictionary(Of String, INDArray))
			Return output().data(iterator).output(outputs).execBatches()
		End Function

		''' <summary>
		''' Set up for an inference operation using OutputConfig.
		''' Supports the setting of variables to output, the input data (<seealso cref="MultiDataSetIterator"/> or <seealso cref="DataSetIterator"/>),
		''' and additional listeners.
		''' Has exec methods to get results in batches or concatenated, or to get results when there is only
		''' a single output (again in batches or concatenated).
		''' <para>
		''' Also supports using <seealso cref="SDVariable"/> instances instead of variable names.
		''' 
		''' <br><br>
		''' Example: get the output of pred, with batches concatenated together
		''' <pre>
		'''     {@code
		'''     SameDiff sd = ...;
		'''     MultiDataSet data = ...;
		''' 
		'''     INDArray out = sd.output()
		'''         .data(data)
		'''         .output("pred")
		'''         .outputSingle();
		'''     }
		''' </pre>
		''' </para>
		''' </summary>
		Public Overridable Function output() As OutputConfig
			Return New OutputConfig(Me)
		End Function

		''' <summary>
		''' Helper method to run inference.  Also used for validation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private List<Map<String, org.nd4j.linalg.api.ndarray.INDArray>> outputHelper(org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator iterator, At at, @NonNull List<Listener> listeners, @NonNull String... outputs)
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Function outputHelper(ByVal iterator As MultiDataSetIterator, ByVal at_Conflict As At, ByVal listeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IList(Of IDictionary(Of String, INDArray))
			Preconditions.checkState(trainingConfig_Conflict IsNot Nothing, "Training config has not been set")

			Dim activeListeners As IList(Of Listener) = New List(Of Listener)()

			For Each l As Listener In listeners
				If l.isActive(at_Conflict.operation()) Then
					activeListeners.Add(l)
				End If
			Next l

			For Each l As Listener In Me.listeners_Conflict
				If l.isActive(at_Conflict.operation()) Then
					activeListeners.Add(l)
				End If
			Next l

			validateListenerActivations(activeListeners, at_Conflict.operation())

			For Each l As Listener In activeListeners
				l.operationStart(Me, at_Conflict.operation())
			Next l

			Dim hasListeners As Boolean = activeListeners.Count > 0

			Dim neededOutputs As IList(Of String)

			If outputs IsNot Nothing AndAlso outputs.Length <> 0 Then
				neededOutputs = New List(Of String) From {outputs}
			Else
				neededOutputs = getLossVariables()
			End If

			Dim neededOutputsArr() As String = CType(neededOutputs, List(Of String)).ToArray()

			Dim predictions As IList(Of IDictionary(Of String, INDArray)) = New List(Of IDictionary(Of String, INDArray))()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() AndAlso iterator.resetSupported() Then
				iterator.reset()
			End If

			Dim requiredVars As ISet(Of String) = New HashSet(Of String)()

			For Each l As Listener In activeListeners
				If at_Conflict.operation() = Operation.TRAINING_VALIDATION Then
					requiredVars.addAll(l.requiredVariables(Me).validationVariables())
				Else
					requiredVars.addAll(l.requiredVariables(Me).inferenceVariables())
				End If
			Next l

			Do While iterator.MoveNext()
				Dim dataStart As Long = If(hasListeners, DateTimeHelper.CurrentUnixTimeMillis(), 0)
				Dim ds As MultiDataSet = iterator.Current
				Dim dataEnd As Long = If(hasListeners, DateTimeHelper.CurrentUnixTimeMillis(), 0)
				Dim placeholderMap As IDictionary(Of String, INDArray) = toPlaceholderMap(ds)

				If hasListeners Then

					For Each l As Listener In activeListeners
						l.iterationStart(Me, at_Conflict, ds, (dataEnd - dataStart))
					Next l

					Dim outs As IDictionary(Of String, INDArray) = directExecHelper(placeholderMap, at_Conflict, ds, requiredVars, activeListeners, neededOutputsArr)

					For Each l As Listener In activeListeners
						l.iterationDone(Me, at_Conflict, ds, Nothing)
					Next l

					predictions.Add(outs)
				Else
					predictions.Add(directExecHelper(placeholderMap, at_Conflict, ds, requiredVars, activeListeners, neededOutputsArr))
				End If
				at_Conflict.setIteration(at_Conflict.iteration() + 1)
			Loop


			For Each l As Listener In activeListeners
				l.operationEnd(Me, at_Conflict.operation())
			Next l

			Return predictions
		End Function

		''' <summary>
		''' Set up for a single batch inference operation using OutputConfig.
		''' Supports the setting of placeholder inputs, outputs, and additional listeners.
		''' Has exec methods to get the single output if only one is requested, or all requested outputs.
		''' <para>
		''' Also supports using <seealso cref="SDVariable"/> instances instead of variable names.
		''' </para>
		''' <para>
		''' Example: get the value of "out" with placeholders x and y
		''' <pre>
		'''     {@code
		'''     SameDiff sd = ...;
		'''     INDArray xValue = ...;
		'''     INDArray yValue = ...;
		'''     SDVariable y = ...;
		''' 
		'''     INDArray outValue = sd.batchOutput()
		'''         .output("out")
		'''         .input("x", xValue)
		'''         .input(y, yValue)
		'''         .outputSingle();
		'''     }
		''' </pre>
		''' </para>
		''' </summary>
		Public Overridable Function batchOutput() As BatchOutputConfig
			Return New BatchOutputConfig(Me)
		End Function

		''' <summary>
		''' Do inference for all variables for a single batch.
		''' <para>
		''' See <seealso cref="output(Map, List, String...)"/>.
		''' </para>
		''' <para>
		''' Special case of <seealso cref="batchOutput()"/>.
		''' </para>
		''' </summary>
		Public Overridable Function outputAll(ByVal placeholders As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)
			Return batchOutput().outputAll().inputs(placeholders).output()
		End Function
		''' <summary>
		''' Do inference for a single variable for a single batch.
		''' <para>
		''' See <seealso cref="output(Map, List, String...)"/>.
		''' </para>
		''' <para>
		''' Special case of <seealso cref="batchOutput()"/>.
		''' </para>
		''' </summary>
		Public Overridable Function outputSingle(ByVal placeholders As IDictionary(Of String, INDArray), ByVal output As String) As INDArray
			Return batchOutput().output(output).inputs(placeholders).outputSingle()
		End Function

		''' <summary>
		''' Do inference for the given variables for a single batch.
		''' <para>
		''' See <seealso cref="output(Map, List, String...)"/>.
		''' </para>
		''' <para>
		''' Special case of <seealso cref="batchOutput()"/>.
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> output(Map<String, org.nd4j.linalg.api.ndarray.INDArray> placeholders, @NonNull List<String> outputs)
		Public Overridable Function output(ByVal placeholders As IDictionary(Of String, INDArray), ByVal outputs As IList(Of String)) As IDictionary(Of String, INDArray)
			Return batchOutput().output(CType(outputs, List(Of String)).ToArray()).inputs(placeholders).output()
		End Function

		''' <summary>
		''' Do inference for the given variables for a single batch.
		''' <para>
		''' See <seealso cref="output(Map, List, String...)"/>.
		''' </para>
		''' <para>
		''' Special case of <seealso cref="batchOutput()"/>.
		''' </para>
		''' </summary>
		Public Overridable Function output(ByVal placeholders As IDictionary(Of String, INDArray), ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return batchOutput().output(outputs).inputs(placeholders).output()
		End Function


		''' <summary>
		''' Do inference for the given variables for a single batch.
		''' <para>
		''' Special case of <seealso cref="batchOutput()"/>.
		''' 
		''' </para>
		''' </summary>
		''' <param name="placeholders"> The values to use for placeholders. </param>
		''' <param name="listeners">    Additional listeners to use during this operation. </param>
		''' <param name="outputs">      The variables to output and return. </param>
		Public Overridable Function output(ByVal placeholders As IDictionary(Of String, INDArray), ByVal listeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Return batchOutputHelper(placeholders, listeners, Operation.INFERENCE, outputs)
		End Function

		Protected Friend Overridable Function batchOutputHelper(ByVal placeholders As IDictionary(Of String, INDArray), ByVal listeners As IList(Of Listener), ByVal operation As Operation, ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			Dim activeListeners As IList(Of Listener) = New List(Of Listener)()

			If operation = Nothing Then
				operation = Operation.INFERENCE
			End If

			For Each l As Listener In Me.listeners_Conflict
				If l.isActive(operation) Then
					activeListeners.Add(l)
				End If
			Next l

			If listeners IsNot Nothing Then
				For Each l As Listener In listeners
					If l.isActive(operation) Then
						activeListeners.Add(l)
					End If
				Next l
			End If

			For Each l As Listener In activeListeners
				l.operationStart(Me, operation)
			Next l

			validateListenerActivations(activeListeners, operation)

			Dim ret As IDictionary(Of String, INDArray) = directExecHelper(placeholders, At.defaultAt(operation), Nothing, java.util.Collections.emptyList(), activeListeners, outputs)

			For Each l As Listener In activeListeners
				l.operationEnd(Me, operation)
			Next l
			Return ret
		End Function

		''' <summary>
		''' Do inference for the given variables for a single batch, with training information
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Protected Friend Overridable Function directExecHelper(ByVal placeholders As IDictionary(Of String, INDArray), ByVal at_Conflict As At, ByVal batch As MultiDataSet, ByVal requiredActivations As ICollection(Of String), ByVal activeListeners As IList(Of Listener), ParamArray ByVal outputs() As String) As IDictionary(Of String, INDArray)
			If at_Conflict Is Nothing Then
				at_Conflict = At.defaultAt()
			End If

			Preconditions.checkState(outputs IsNot Nothing AndAlso outputs.Length > 0, "No outputs were specified")
			Dim threadId As Long = Thread.CurrentThread.getId()
			If Not sessions.ContainsKey(threadId) Then
				log.info("Creating new InferenceSession for thread {}", threadId)
				sessions(threadId) = New InferenceSession(Me)
			End If

			Dim phNames As IList(Of String) = inputs()
			If placeholders Is Nothing AndAlso phNames IsNot Nothing Then
				'Maybe user set placeholders before calling exec method?
				placeholders = placeholdersPerThread(Thread.CurrentThread.getId())
			End If

			'Placeholder validation is performed in InferenceSession

			Dim [is] As InferenceSession = sessions(threadId)
			Return [is].output(If(outputs Is Nothing, java.util.Collections.emptyList(), java.util.Arrays.asList(outputs)), placeholders, batch, requiredActivations, activeListeners, at_Conflict)
		End Function

		''' <summary>
		''' See <seealso cref="one(String, DataType, Integer...)"/>.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' Uses the DataType of the Nd4j default floating point type (<seealso cref="Nd4j.defaultFloatingPointType()"/>).
		''' </summary>
		Public Overridable Function one(ByVal name As String, ParamArray ByVal shape() As Integer) As SDVariable
			Return one(name, Nd4j.defaultFloatingPointType(), shape)
		End Function

		''' <summary>
		''' See <seealso cref="one(String, DataType, Long...)"/>.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' Uses the DataType of the Nd4j default floating point type (<seealso cref="Nd4j.defaultFloatingPointType()"/>).
		''' </summary>
		Public Overridable Function one(ByVal name As String, ParamArray ByVal shape() As Long) As SDVariable
			Return one(name, Nd4j.defaultFloatingPointType(), shape)
		End Function


		''' <summary>
		''' Create a new variable with the specified shape, with all values initialized to 1.0.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' </summary>
		''' <param name="name">  the name of the variable to create </param>
		''' <param name="shape"> the shape of the array to be created </param>
		''' <returns> the created variable </returns>
		Public Overridable Function one(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Integer) As SDVariable
			Return one(name, dataType, ArrayUtil.toLongArray(shape))
		End Function

		''' <summary>
		''' Create a new variable with the specified shape, with all values initialized to 1.0.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' </summary>
		''' <param name="name">  the name of the variable to create </param>
		''' <param name="shape"> the shape of the array to be created </param>
		''' <returns> the created variable </returns>
		Public Overridable Function one(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Return constant(name, Nd4j.ones(dataType, shape))
		End Function

		''' <summary>
		''' See <seealso cref="zero(String, DataType, Long...)"/>.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' Uses the DataType of the Nd4j default floating point type (<seealso cref="Nd4j.defaultFloatingPointType()"/>).
		''' </summary>
		Public Overridable Function zero(ByVal name As String, ParamArray ByVal shape() As Long) As SDVariable
			Return zero(name, Nd4j.defaultFloatingPointType(), shape)
		End Function

		''' <summary>
		''' See <seealso cref="zero(String, DataType, Integer...)"/>.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' Uses the DataType of the Nd4j default floating point type (<seealso cref="Nd4j.defaultFloatingPointType()"/>).
		''' </summary>
		Public Overridable Function zero(ByVal name As String, ParamArray ByVal shape() As Integer) As SDVariable
			Return zero(name, Nd4j.defaultFloatingPointType(), shape)
		End Function

		''' <summary>
		''' Create a new variable with the specified shape, with all values initialized to 0.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' </summary>
		''' <param name="name">  the name of the variable to create </param>
		''' <param name="shape"> the shape of the array to be created </param>
		''' <returns> the created variable </returns>
		Public Overridable Function zero(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Return constant(name, Nd4j.zeros(dataType, shape))
		End Function

		''' <summary>
		''' Create a new variable with the specified shape, with all values initialized to 0.
		''' Creates a constant - i.e., CONSTANT type SDVariable.
		''' </summary>
		''' <param name="name">  the name of the variable to create </param>
		''' <param name="shape"> the shape of the array to be created </param>
		''' <returns> the created variable </returns>
		Public Overridable Function zero(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Integer) As SDVariable
			Return zero(name, dataType, ArrayUtil.toLongArray(shape))
		End Function

		''' <summary>
		''' Create an SDVariable with a fixed/constant value, with a generated name<br>
		''' Constants are not modified by training/backprop. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="constant"> Value for the constant SDVariable </param>
		''' <returns> The created variable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable constant(@NonNull INDArray constant)
'JAVA TO VB CONVERTER NOTE: The parameter constant was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function constant(ByVal constant_Conflict As INDArray) As SDVariable
			Return Me.constant(NewVarName, constant_Conflict)
		End Function

		''' <summary>
		''' Create an SDVariable with a fixed/constant value<br>
		''' Constants are not modified by training/backprop. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">     Name of the constant SDVariable </param>
		''' <param name="constant"> Value for the constant SDVariable </param>
		''' <returns> The created variable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable constant(String name, @NonNull INDArray constant)
'JAVA TO VB CONVERTER NOTE: The parameter constant was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function constant(ByVal name As String, ByVal constant_Conflict As INDArray) As SDVariable
			Preconditions.checkState(Not variables_Conflict.ContainsKey(name), "Variable with name ""%s"" already exists", name)
			If name Is Nothing OrElse name.Length < 1 Then
				name = NewVarName
			End If
			If constant_Conflict.isView() Then
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					constant_Conflict = constant_Conflict.dup()
				End Using
			End If

			Dim v As New SDVariable(name, VariableType.CONSTANT, Me, constant_Conflict.shape(), constant_Conflict.dataType())
			name = v.name()
			variables(name) = Variable.builder().name(name).variable(v).build()
			constantArrays.setArray(name, constant_Conflict)
			Return v
		End Function

		''' <summary>
		''' Create a a placeholder variable. Placeholders are variables that expect an array to be provided during training
		''' and inference.<br>
		''' For example, the SDVariables for your input/features and labels should be placeholders.<br>
		''' See also: <seealso cref="VariableType"/>
		''' </summary>
		''' <param name="name">     the name of the variable </param>
		''' <param name="dataType"> Data type of the new placeholder </param>
		''' <param name="shape">    the shape of the variable if any </param>
		''' <returns> SDVariable placeholder </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable placeHolder(@NonNull String name, org.nd4j.linalg.api.buffer.DataType dataType, long... shape)
		Public Overridable Function placeHolder(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Preconditions.checkState(Not variables_Conflict.ContainsKey(name), "Variable already exists with name %s", name)
			Dim ret As New SDVariable(name, VariableType.PLACEHOLDER, Me, shape, dataType)
			variables(name) = Variable.builder().name(name).variable(ret).build()
			Return ret
		End Function

		''' <summary>
		''' Variable initialization with a specified <seealso cref="WeightInitScheme"/>
		''' This method creates VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">             the name of the variable </param>
		''' <param name="shape">            the shape of the array to be created </param>
		''' <param name="weightInitScheme"> the weight initialization scheme </param>
		''' <returns> the created variable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable var(@NonNull String name, @NonNull WeightInitScheme weightInitScheme, @NonNull org.nd4j.linalg.api.buffer.DataType dataType, @NonNull long... shape)
		Public Overridable Function var(ByVal name As String, ByVal weightInitScheme As WeightInitScheme, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Return var(name, VariableType.VARIABLE, weightInitScheme, dataType, shape)
		End Function

		''' <summary>
		''' Variable initialization with a specified <seealso cref="WeightInitScheme"/>
		''' This method creates VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">             the name of the variable </param>
		''' <param name="variableType">     the SameDiff variable type of the variable (e.g. CONSTANT, PLACEHOLDER, etc.) </param>
		''' <param name="weightInitScheme"> the weight initialization scheme </param>
		''' <param name="dataType">         the data type of the variable (float, int, etc) </param>
		''' <param name="shape">            the shape of the array to be created </param>
		''' <returns> the created variable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable var(@NonNull String name, @NonNull VariableType variableType, org.nd4j.weightinit.WeightInitScheme weightInitScheme, org.nd4j.linalg.api.buffer.DataType dataType, long... shape)
		Public Overridable Function var(ByVal name As String, ByVal variableType As VariableType, ByVal weightInitScheme As WeightInitScheme, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			If shape IsNot Nothing Then
				For Each l As Long In shape
					Preconditions.checkArgument(l <> 0, "Cannot create variable with a shape that contains zeros (empty array shape) - got shape %s", shape)
				Next l
			End If

			If name Is Nothing OrElse name.Length < 1 Then
				name = NewVarName
			Else
				name = generateNewVarName(name, 0)
			End If

			If variables_Conflict.ContainsKey(name) Then
				If nameScopes.Count = 0 Then
					Throw New System.ArgumentException("Another variable with the name " & name & " already exists (current name scope: """ & currentNameScope() & """")
				Else
					Throw New System.ArgumentException("Another variable with the name " & name & " already exists.")
				End If
			End If

			Preconditions.checkState(variableType <> VariableType.VARIABLE OrElse weightInitScheme IsNot Nothing, "A weight initalization scheme must be provided" & " when creating a VARIABLE type SDVariables - variable name: ""%s""", name)

			Dim ret As New SDVariable(name, variableType, Me, shape, dataType)
			addVariable(ret)

			If variableType = VariableType.VARIABLE Then
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
					Dim vArr As INDArray = weightInitScheme.create(dataType, shape)
					variablesArrays.setArray(name, vArr)
				End Using
			End If

			Return ret
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the given shape and name<br>
		''' The underlying array will be initialized using the specified weight initilization scheme<br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">             the name of the variable </param>
		''' <param name="shape">            the shape of the variable </param>
		''' <param name="weightInitScheme"> Weight initialization scheme to use to initialize the underlying array </param>
		''' <returns> the created variable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable var(@NonNull String name, @NonNull LongShapeDescriptor shape, org.nd4j.weightinit.WeightInitScheme weightInitScheme)
		Public Overridable Function var(ByVal name As String, ByVal shape As LongShapeDescriptor, ByVal weightInitScheme As WeightInitScheme) As SDVariable
			Return var(name, weightInitScheme, shape.dataType(), shape.getShape())
		End Function


		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the given shape and name<br>
		''' Any array will be generated with all zeros for the values<br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">  the name of the variable </param>
		''' <param name="shape"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Preconditions.checkNotNull(shape IsNot Nothing, "Invalid shape: shape may not be null")
			If Shape.isPlaceholderShape(shape) Then
				Return placeHolder(name, dataType, shape)
			End If
			Return var(name, New ZeroInitScheme(), dataType, shape)
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the given shape and name<br>
		''' Any array will be generated with all zeros for the values<br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">      the name of the variable </param>
		''' <param name="shapeDesc"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal name As String, ByVal shapeDesc As LongShapeDescriptor) As SDVariable
			Preconditions.checkNotNull(shapeDesc IsNot Nothing, "Invalid shape: shape may not be null")
			Return var(name, shapeDesc, New ZeroInitScheme())
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the given shape and name<br>
		''' Any array will be generated with all zeros for the values. Data type will be given by <seealso cref="Nd4j.defaultFloatingPointType()"/><br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">  the name of the variable </param>
		''' <param name="shape"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal name As String, ParamArray ByVal shape() As Integer) As SDVariable
			Return var(name, Nd4j.defaultFloatingPointType(), shape)
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the given shape and name<br>
		''' Any array will be generated with all zeros for the values. Data type will be given by <seealso cref="Nd4j.defaultFloatingPointType()"/><br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">  the name of the variable </param>
		''' <param name="shape"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal name As String, ParamArray ByVal shape() As Long) As SDVariable
			Return var(name, Nd4j.defaultFloatingPointType(), shape)
		End Function

		''' <summary>
		''' Variable initialization with a specified <seealso cref="WeightInitScheme"/>. Data type will be given by <seealso cref="Nd4j.defaultFloatingPointType()"/><br>
		''' This method creates VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="name">             the name of the variable </param>
		''' <param name="shape">            the shape of the array to be created </param>
		''' <param name="weightInitScheme"> the weight initialization scheme </param>
		''' <returns> the created variable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable var(@NonNull String name, @NonNull WeightInitScheme weightInitScheme, @NonNull long... shape)
		Public Overridable Function var(ByVal name As String, ByVal weightInitScheme As WeightInitScheme, ParamArray ByVal shape() As Long) As SDVariable
			Return var(name, weightInitScheme, Nd4j.defaultFloatingPointType(), shape)
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the given shape and name<br>
		''' Any array will be generated with all zeros for the values<br>
		''' </summary>
		''' <param name="name">  the name of the variable </param>
		''' <param name="shape"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal name As String, ByVal dataType As DataType, ParamArray ByVal shape() As Integer) As SDVariable
			Preconditions.checkNotNull(shape, "Invalid shape: shape may not be null")
			If Shape.isPlaceholderShape(shape) Then
				Return placeHolder(name, dataType, ArrayUtil.toLongArray(shape))
			End If
			Return var(name, New ZeroInitScheme(), dataType, ArrayUtil.toLongArray(shape))
		End Function


		''' <summary>
		''' Initialize a <seealso cref="SDVariable"/> reference tying this variable to this samediff instance.
		''' <para>
		''' <seealso cref="NDArraySupplierInitScheme"/> is used to ensure that if the array is allocated anywhere
		''' and <seealso cref="SameDiff"/> instance to exist as a copy of the variable.
		''' 
		''' </para>
		''' </summary>
		''' <param name="v"> Variable
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable var(@NonNull final SDVariable v)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Overridable Function var(ByVal v As SDVariable) As SDVariable
			If variables_Conflict.ContainsKey(v.name()) AndAlso variables(v.name()).getVariable().getArr() IsNot Nothing Then
				Return variables(v.name()).getVariable()
			End If

			If v.name() Is Nothing OrElse v.name().Length < 1 Then
				Throw New System.ArgumentException("Name for variable must be defined")
			End If

			Dim vt As VariableType = v.getVariableType()
			Dim s As NDArraySupplierInitScheme = Nothing
			Select Case vt
				Case org.nd4j.autodiff.samediff.VariableType.VARIABLE
					Dim r As New SDVariable(v.name(), v.getVariableType(), Me, v.Shape, v.dataType())
					addVariable(r)
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
						variablesArrays.setArray(v.name(), v.Arr.dup())
					End Using
					Return r
				Case org.nd4j.autodiff.samediff.VariableType.ARRAY
					Dim ret As New SDVariable(v.name(), v.getVariableType(), Me, v.Shape, v.dataType())
					Return addVariable(ret)
				Case org.nd4j.autodiff.samediff.VariableType.CONSTANT
					Return constant(v.name(), v.Arr)
				Case org.nd4j.autodiff.samediff.VariableType.PLACEHOLDER
					Return placeHolder(v.name(), v.dataType(), v.placeholderShape())
				Case Else
					Throw New Exception("Unknown/not supported variable type: " & vt)
			End Select
		End Function

		Private ReadOnly Property NewVarName As String
			Get
				Return generateNewVarName("sd_var", 0, False)
			End Get
		End Property

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the specified shape and a generated name<br>
		''' Any array will be generated with all zeros for the values<br>
		''' This method creates a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="shape"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal dataType As DataType, ParamArray ByVal shape() As Integer) As SDVariable
			Return var(NewVarName, dataType, shape)
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the specified shape and a generated name<br>
		''' Any array will be generated with all zeros for the values<br>
		''' This method creates a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="shape"> the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Return var(NewVarName, dataType, shape)
		End Function

		''' <summary>
		''' Creates a <seealso cref="SDVariable"/> with the specified shape and a generated name. The associated array will
		''' then be generated using the specified weight initialization scheme
		''' </summary>
		''' <param name="weightInitScheme"> The weight initialization scheme to use when generating an INDArray </param>
		''' <param name="shape">            the shape of the variable </param>
		''' <returns> the created variable </returns>
		Public Overridable Function var(ByVal weightInitScheme As WeightInitScheme, ByVal dataType As DataType, ParamArray ByVal shape() As Long) As SDVariable
			Return var(NewVarName, weightInitScheme, dataType, shape)
		End Function

		''' <summary>
		''' Create an <seealso cref="SDVariable"/> with a generated name, and assocate the specified array with it.<br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="arr"> Array to associate with the new variable </param>
		''' <returns> New SDVariable </returns>
		''' <seealso cref= #var(String, INDArray) </seealso>
		Public Overridable Function var(ByVal arr As INDArray) As SDVariable
			Return var(NewVarName, arr)
		End Function

		''' <summary>
		''' Create an <seealso cref="SDVariable"/> with the specified name, and associate the specified array with it<br>
		''' This is a VARIABLE type SDVariable - i.e., must be floating point, and is a trainable parameter. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="arr"> Array to associate with the new variable </param>
		''' <returns> New SDVariable with the specified name and array </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable var(String name, @NonNull INDArray arr)
		Public Overridable Function var(ByVal name As String, ByVal arr As INDArray) As SDVariable
			If variables_Conflict.ContainsKey(name) AndAlso variables(name).getVariable().getArr() IsNot Nothing Then
				Throw New System.ArgumentException("Another variable with the name " & name & " already exists.")
			End If
			Preconditions.checkState(arr.dataType().isFPType(), "Cannot create variable with non-floating point type:" & " provided array has datatype %s. Variables must be floating point type to be trainable by backpropagation." & vbLf & "For non floating point types, these should be created as placeholders or constants instead.", arr.dataType())
			Preconditions.checkArgument(Not arr.isEmpty(), "Empty arrays cannot be used when creating variables. Array shape: %ndShape", arr)

			If name Is Nothing OrElse name.Length < 1 Then
				name = NewVarName
			End If

			Dim duped As Boolean = False
			If arr.isAttached() Then
				arr = arr.detach()
				duped = True
			End If

			If Not duped Then
				For Each s As String In variablesArrays.arrayNames()
					If variablesArrays.getArray(s) Is arr Then 'Check for exact same object, to avoid array reuse (can result in unexpected behaviour)
						arr = arr.dup()
						Exit For
					End If
				Next s
			End If


			Dim ret As New SDVariable(name, VariableType.VARIABLE, Me, arr.shape(), arr.dataType())
			associateArrayWithVariable(arr, ret)

			addVariable(ret)
			Return ret
		End Function

		''' <summary>
		''' Convert the specified variable to a constant. This is equivalent to "freezing" a variable so that it's value
		''' won't be changed by further training.<br>
		''' This can only be done for variables and placeholders, not ARRAY type variables (which are usually network activations).
		''' As a constant, this variable will no longer be modified by any subsequent training.<br>
		''' See also: <seealso cref="VariableType"/>
		''' </summary>
		''' <param name="variable"> Variable to convert to a constant </param>
		''' <returns> The (now constant) SDVariable </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable convertToConstant(@NonNull SDVariable variable)
		Public Overridable Function convertToConstant(ByVal variable As SDVariable) As SDVariable
			convertToConstants(Collections.singletonList(variable))
			Return variable
		End Function

		''' <summary>
		''' Convert all of the specified variables to constants. This is equivalent to "freezing" the variables so that their values
		''' won't be changed by further training.<br>
		''' This can only be done for variables and placeholders, not ARRAY type variables (which are usually network activations).
		''' As constants, these variables will no longer be modified by any subsequent training.<br>
		''' See also: <seealso cref="VariableType"/>
		''' </summary>
		''' <param name="variables"> Variables to convert to constants </param>
		''' <returns> The (now constant) SDVariables </returns>
		Public Overridable Sub convertToConstants(ByVal variables As IList(Of SDVariable))
			If variables.Count = 0 Then
				Return
			End If
			Dim allConst As Boolean = True
			For Each variable As SDVariable In variables
				If variable.getVariableType() <> VariableType.CONSTANT Then
					allConst = False
					Preconditions.checkState(variable.getVariableType() <> VariableType.ARRAY, "Cannot convert variable of type ARRAY to a constant: %s", variable)
				End If
			Next variable
			If allConst Then
				Return 'No op
			End If

			'Remove all sessions in case they have any cached arrays/state
			sessions.Clear()

			'If gradient function has been defined, remove it (so it will be recreated later)
			sameDiffFunctionInstances.Remove(GRAD_FN_KEY)

			For Each variable As SDVariable In variables
				Dim n As String = variable.name()
				Dim arr As INDArray = variable.Arr
				Preconditions.checkNotNull(arr, "Could not get array for variable %s: if this is a placeholder, use SDVariable.setArray before converting", variable)

				constantArrays.setArray(n, arr) 'DeviceLocal with delayed initialization, in case we don't actually need multiple threads
				variablesArrays.removeArray(n)
				If placeholdersPerThread.Count > 0 Then
					For Each m As IDictionary(Of String, INDArray) In placeholdersPerThread.Values
						m.Remove(n)
					Next m
				End If

				variable.setVariableType(VariableType.CONSTANT)
			Next variable


			If trainingConfig_Conflict IsNot Nothing AndAlso initializedTraining Then
				'Remove updater state for now constant variables
				For Each v As SDVariable In variables
					Dim gu As GradientUpdater = updaterMap.Remove(v.name())
					Dim m As IDictionary(Of String, INDArray) = If(gu Is Nothing, Nothing, gu.getState())
					If m IsNot Nothing Then
						For Each arr As INDArray In m.Values
							If arr.closeable() Then
								arr.close()
							End If
						Next arr
					End If

					'Also check dataset feature/label mapping -  remove any placeholders here...
					If trainingConfig_Conflict.getDataSetFeatureMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetFeatureMapping().contains(v.name()) Then
						Dim newFM As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetFeatureMapping()) 'New list in case of immutable list
						newFM.Remove(v.name())
						trainingConfig_Conflict.setDataSetFeatureMapping(newFM)
					End If

					If trainingConfig_Conflict.getDataSetLabelMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetLabelMapping().contains(v.name()) Then
						Dim newLM As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetLabelMapping())
						newLM.Remove(v.name())
						trainingConfig_Conflict.setDataSetLabelMapping(newLM)
					End If

					If trainingConfig_Conflict.getDataSetFeatureMaskMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetFeatureMaskMapping().contains(v.name()) Then
						Dim newFMM As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetFeatureMaskMapping())
						newFMM.Remove(v.name())
						trainingConfig_Conflict.setDataSetFeatureMaskMapping(newFMM)
					End If

					If trainingConfig_Conflict.getDataSetLabelMaskMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetLabelMaskMapping().contains(v.name()) Then
						Dim newLMM As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetLabelMaskMapping())
						newLMM.Remove(v.name())
						trainingConfig_Conflict.setDataSetLabelMaskMapping(newLMM)
					End If
				Next v
			End If
		End Sub

		''' <summary>
		''' Convert the specified variable to a VARIABLE type SDVariable.<br>
		''' This can only be done for constants and placeholders, not ARRAY type variables (which are usually network activations).
		''' As a variable, this variable will modified during any subsequent training.<br>
		''' See also: <seealso cref="VariableType"/>
		''' </summary>
		''' <returns> This variable (now a variable type SDVariable) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable convertToVariable(@NonNull SDVariable constant)
		Public Overridable Function convertToVariable(ByVal constant As SDVariable) As SDVariable
			Preconditions.checkState(constant.dataType().isFPType(), "Only floating point SDVariables can be converted to variables," & " datatype of %s is %s", constant.name(), constant.dataType())
			convertToVariables(Collections.singletonList(constant))
			Return constant
		End Function

		''' <summary>
		''' Convert the specified variables to VARIABLE type SDVariables.<br>
		''' This can only be done for constants and placeholders, not ARRAY type variables (which are usually network activations).
		''' As variables, this variable will modified during any subsequent training.<br>
		''' See also: <seealso cref="VariableType"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void convertToVariables(@NonNull List<SDVariable> constants)
		Public Overridable Sub convertToVariables(ByVal constants As IList(Of SDVariable))
			If constants.Count = 0 Then
				Return
			End If
			Dim allConst As Boolean = True
			For Each variable As SDVariable In constants
				If variable.getVariableType() <> VariableType.VARIABLE Then
					allConst = False
				End If
				Preconditions.checkState(variable.getVariableType() <> VariableType.ARRAY, "Cannot convert variable of type ARRAY to a variable: %s", variable)
			Next variable
			If allConst Then
				Return 'No op
			End If

			'Remove all sessions in case they have any cached arrays/state
			sessions.Clear()

			'If gradient function has been defined, remove it (so it will be recreated later)
			sameDiffFunctionInstances.Remove(GRAD_FN_KEY)

			For Each variable As SDVariable In constants
				Dim n As String = variable.name()
				Dim arr As INDArray = variable.Arr
				Preconditions.checkNotNull(arr, "Could not get array for variable %s: if this is a placeholder, use SDVariable.setArray before converting", variable)

				variablesArrays.setArray(n, arr) 'DeviceLocal with delayed initialization, in case we don't actually need multiple threads
				constantArrays.removeArray(n)
				If placeholdersPerThread.Count > 0 Then
					For Each m As IDictionary(Of String, INDArray) In placeholdersPerThread.Values
						m.Remove(n)
					Next m
				End If

				variable.setVariableType(VariableType.VARIABLE)
			Next variable


			'For training: need to add new updater state
			If trainingConfig_Conflict IsNot Nothing AndAlso initializedTraining Then
				'Add updater state for this variable: updaterState, updaterViews, updaterMap
				For Each v As SDVariable In constants
					If Not updaterMap.ContainsKey(v.name()) Then
						'Create new updater state
						Dim arr As INDArray = v.Arr
						Dim thisSize As Long = trainingConfig_Conflict.getUpdater().stateSize(arr.length())
						If thisSize > 0 Then
							Dim stateArr As INDArray = Nd4j.create(arr.dataType(), 1, thisSize)
							Dim u As GradientUpdater = trainingConfig_Conflict.getUpdater().instantiate(stateArr, False)
							u.setStateViewArray(stateArr, arr.shape(), arr.ordering(), True) 'TODO eventually this should be 1 call...
							updaterMap(v.name()) = u
						Else
							Dim u As GradientUpdater = trainingConfig_Conflict.getUpdater().instantiate(DirectCast(Nothing, INDArray), True)
							updaterMap(v.name()) = u
						End If
					End If
				Next v
			End If
		End Sub

		''' <summary>
		''' Convert the datatypes of the specified constants, placeholders and variables.<br>
		''' After conversion, the downstream datatypes are changed.
		''' For example, {@code z(float) = x(float)+y(float)}, changing both x and y to double results in {@code z(double) = x(double)+y(double)}
		''' without doing anything to change z's datatype directly (z datatype is inferred from x + y + add op).<br>
		''' ARRAY type SDVariables cannot be converted directly, as their datatypes are determined by the function +
		''' input datatypes.<b>
		''' Note that this method should be used with caution: incorrect datatype modifications may leave your network
		''' in an incorrect state. For example, {@code op(x(float),y(float)) -> op(x(double),y(float))} may not be
		''' supported by all ops.
		''' </summary>
		''' <param name="dataTypeMap"> Map of SDVariables to change the datatype for. Key = SDVariable name, Value = new datatype </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void convertDataTypes(@NonNull Map<String, org.nd4j.linalg.api.buffer.DataType> dataTypeMap)
		Public Overridable Sub convertDataTypes(ByVal dataTypeMap As IDictionary(Of String, DataType))
			If dataTypeMap.Count = 0 Then
				Return
			End If

			'First: check these are all either constants, variables or placeholders.
			For Each e As KeyValuePair(Of String, DataType) In dataTypeMap.SetOfKeyValuePairs()
				Dim s As String = e.Key
				Preconditions.checkState(variables_Conflict.ContainsKey(s), "Cannot change datatype of variable ""%s"": No variable with this name exists", s)
				Dim v As SDVariable = variables(s).getVariable()
				Preconditions.checkState(v.getVariableType() <> VariableType.ARRAY, "Cannot change datatype of ARRAY type variable ""%s"": " & "datatype of ARRAY type variables is determined by the datatypes of their inputs plus corresponding ")
				If v.getVariableType() <> VariableType.PLACEHOLDER Then
					'Can't convert constant or variable between numerical and non-numerical type (not possible to cast)
					Preconditions.checkState(v.dataType().isNumerical() = e.Value.isNumerical(), "Cannot convert variables between numerical " & "and non-numerical types: attempting to convert variable ""%s"" from %s to %s", e.Key, v.dataType(), e.Value)
				End If
			Next e

			Dim anyChanged As Boolean = False
			For Each e As KeyValuePair(Of String, DataType) In dataTypeMap.SetOfKeyValuePairs()
				Dim s As String = e.Key
				Dim d As DataType = e.Value
				Dim v As SDVariable = variables(s).getVariable()
				If v.dataType() = d Then
					Continue For 'No-op
				End If

				v.setDataType(d)

				Select Case v.getVariableType()
					Case VARIABLE
						Dim arr As INDArray = variablesArrays.removeArray(e.Key)
						Dim newArr As INDArray = arr.castTo(d)
						variablesArrays.setArray(e.Key, newArr) 'DeviceLocal with delayed initialization, in case we don't actually need multiple threads
					Case CONSTANT
						Dim arr2 As INDArray = constantArrays.removeArray(e.Key)
						Dim newArr2 As INDArray = arr2.castTo(d)
						constantArrays.setArray(e.Key, newArr2) 'DeviceLocal with delayed initialization, in case we don't actually need multiple threads
					Case PLACEHOLDER
						Dim m As IDictionary(Of String, INDArray) = placeholdersPerThread(Thread.CurrentThread.getId())
						If m IsNot Nothing AndAlso m.ContainsKey(e.Key) Then
							m(e.Key) = m(e.Key).castTo(d)
						End If
					Case Else
						Throw New System.InvalidOperationException("Cannot convert array type variable") 'Should never happen
				End Select


				anyChanged = True
			Next e

			If anyChanged Then
				sessions.Clear()

				'Recalculate datatypes of outputs, and dynamically update them
				Dim allSeenOps As ISet(Of String) = New HashSet(Of String)()
				Dim queueOps As New LinkedList(Of String)()

				For Each s As String In dataTypeMap.Keys
					Dim v As Variable = variables(s)
					v.getVariable().setDataType(dataTypeMap(s))
					Dim inToOp As IList(Of String) = v.getInputsForOp()
					If inToOp IsNot Nothing Then
						For Each op As String In inToOp
							If Not allSeenOps.Contains(op) Then
								allSeenOps.Add(op)
								queueOps.AddLast(op)
							End If
						Next op
					End If
				Next s

				Do While queueOps.Count > 0
					Dim op As String = queueOps.RemoveFirst()
					Dim o As SameDiffOp = ops(op)
					Dim inVars As IList(Of String) = o.getInputsToOp()
					Dim inDTypes As IList(Of DataType) = New List(Of DataType)()
					If inVars IsNot Nothing Then
						For Each s As String In inVars
							Dim v As SDVariable = variables(s).getVariable()
							inDTypes.Add(v.dataType())
						Next s
					End If
					Dim outDtypes As IList(Of DataType) = o.Op.calculateOutputDataTypes(inDTypes)
					Dim outVars As IList(Of String) = o.getOutputsOfOp()
					For i As Integer = 0 To outVars.Count - 1
						Dim varName As String = outVars(i)
						Dim var As Variable = variables(varName)
						Dim v As SDVariable = var.getVariable()
						v.setDataType(outDtypes(i))

						'Also update queue
						If var.getInputsForOp() IsNot Nothing Then
							For Each opName As String In var.getInputsForOp()
								If Not allSeenOps.Contains(opName) Then
									allSeenOps.Add(opName)
									queueOps.AddLast(opName)
								End If
							Next opName
						End If
					Next i
				Loop
			End If
		End Sub

		''' <summary>
		''' Rename the specified variable to the new name.
		''' Note here we also specify the op.
		''' Sometimes, ops have multiple outputs and after the first rename of the variable
		''' we lose the reference to the correct op to modify. </summary>
		''' <param name="opToReName">  the op to rename </param>
		''' <param name="from"> The variable to rename - this variable must exist </param>
		''' <param name="to">   The new name for the variable - no variable with this name must already exist </param>
		Public Overridable Sub renameVariable(ByVal opToReName As SameDiffOp, ByVal from As String, ByVal [to] As String)
			Preconditions.checkState(variables_Conflict.ContainsKey(from), "Cannot rename variable ""%s"": no variable with this name exists", from)
			Preconditions.checkState(Not variables_Conflict.ContainsKey([to]), "Cannot rename variable ""%s"" to name ""%s"": a variable with name ""%s"" already exists", from, [to], [to])

			Dim v As Variable = variables(from)
			v.setName([to])
			v.getVariable().setVarName([to])
			If v.getInputsForOp() IsNot Nothing Then
				For Each opName As String In v.getInputsForOp()
					Dim op As SameDiffOp = ops(opName)
					Dim newInputs As IList(Of String) = New List(Of String)(op.getInputsToOp())
					Do While newInputs.Contains(from)
						newInputs(newInputs.IndexOf(from)) = [to]
					Loop

					op.InputsToOp = newInputs
				Next opName
			End If

			If v.getControlDepsForOp() IsNot Nothing Then
				For Each opName As String In v.getControlDepsForOp()
					Dim op As SameDiffOp = ops(opName)
					Dim newCDs As IList(Of String) = New List(Of String)(op.getControlDeps())
					Do While newCDs.Contains(from)
						newCDs(newCDs.IndexOf(from)) = [to]
					Loop
					op.ControlDeps = newCDs
				Next opName
			End If

			If v.getControlDepsForVar() IsNot Nothing Then
				For Each varName As String In v.getControlDepsForVar()
					Dim var As Variable = variables(varName)
					Dim newCDs As IList(Of String) = New List(Of String)(var.getControlDeps())
					Do While newCDs.Contains(from)
						newCDs(newCDs.IndexOf(from)) = [to]
					Loop
					var.setControlDeps(newCDs)
				Next varName
			End If

			If v.getControlDeps() IsNot Nothing Then
				For Each varName As String In v.getControlDeps()
					Dim var As Variable = variables(varName)
					Dim newCDsFor As IList(Of String) = New List(Of String)(var.getControlDepsForVar())
					Do While newCDsFor.Contains(from)
						newCDsFor(newCDsFor.IndexOf(from)) = [to]
					Loop
					var.setControlDepsForVar(newCDsFor)
				Next varName
			End If

			If v.getOutputOfOp() IsNot Nothing Then
				Dim op As SameDiffOp = ops(v.getOutputOfOp())
				Dim newOuts As IList(Of String) = New List(Of String)(op.getOutputsOfOp())
				Do While newOuts.Contains(from)
					newOuts(newOuts.IndexOf(from)) = [to]
				Loop
				op.OutputsOfOp = newOuts
			End If

			variables_Conflict.Remove(from)
			variables([to]) = v

			If v.getVariable().getVariableType() = VariableType.CONSTANT AndAlso constantArrays.hasArray(from) Then
				constantArrays.rename(from, [to])
			End If

			If v.getVariable().getVariableType() = VariableType.VARIABLE AndAlso variablesArrays.hasArray(from) Then
				variablesArrays.rename(from, [to])
			End If

			If v.getVariable().getVariableType() = VariableType.PLACEHOLDER Then
				For Each e As IDictionary(Of String, INDArray) In placeholdersPerThread.Values
					'Not really thread safe - but renaming variables during execution in other threads can never be thread safe :)
					If e IsNot Nothing AndAlso e.ContainsKey(from) Then
						Dim arr As INDArray = e.Remove(from)
						e([to]) = arr
					End If
				Next e
			End If

			If trainingConfig_Conflict IsNot Nothing Then
				If trainingConfig_Conflict.getDataSetFeatureMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetFeatureMapping().contains(from) Then
					Dim l As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetFeatureMapping())
					Do While l.Contains(from)
						l(l.IndexOf(from)) = [to]
					Loop
					trainingConfig_Conflict.setDataSetFeatureMapping(l)
				End If

				If trainingConfig_Conflict.getDataSetLabelMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetLabelMapping().contains(from) Then
					Dim l As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetLabelMapping())
					Do While l.Contains(from)
						l(l.IndexOf(from)) = [to]
					Loop
					trainingConfig_Conflict.setDataSetLabelMapping(l)
				End If

				If trainingConfig_Conflict.getDataSetFeatureMaskMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetFeatureMaskMapping().contains(from) Then
					Dim l As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetFeatureMaskMapping())
					Do While l.Contains(from)
						l(l.IndexOf(from)) = [to]
					Loop
					trainingConfig_Conflict.setDataSetFeatureMaskMapping(l)
				End If

				If trainingConfig_Conflict.getDataSetLabelMaskMapping() IsNot Nothing AndAlso trainingConfig_Conflict.getDataSetLabelMaskMapping().contains(from) Then
					Dim l As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getDataSetLabelMaskMapping())
					Do While l.Contains(from)
						l(l.IndexOf(from)) = [to]
					Loop

					trainingConfig_Conflict.setDataSetLabelMaskMapping(l)
				End If

				If trainingConfig_Conflict.getLossVariables() IsNot Nothing AndAlso trainingConfig_Conflict.getLossVariables().contains(from) Then
					Dim l As IList(Of String) = New List(Of String)(trainingConfig_Conflict.getLossVariables())
					Do While l.Contains(from)
						l(l.IndexOf(from)) = [to]
					Loop
					trainingConfig_Conflict.setLossVariables(l)
				End If
			End If

			For Each sd As SameDiff In sameDiffFunctionInstances.Values
				If sd.hasVariable(from) Then
					sd.renameVariable(from, [to])
				End If
			Next sd

			'Check losses:
			If lossVariables_Conflict.Contains(from) Then
				Dim idx As Integer = lossVariables_Conflict.IndexOf(from)
				lossVariables(idx) = [to]
			End If
		End Sub


		''' <summary>
		''' Rename the specified variable to the new name.
		''' </summary>
		''' <param name="from"> The variable to rename - this variable must exist </param>
		''' <param name="to">   The new name for the variable - no variable with this name must already exist </param>
		Public Overridable Sub renameVariable(ByVal from As String, ByVal [to] As String)
			Dim op As SameDiffOp = ops(stripVarSuffix(from))
			renameVariable(op,from,[to])
		End Sub


		''' <summary>
		''' Remove an argument for a function. Note that if this function does not contain the argument, it will just be a no op.
		''' </summary>
		''' <param name="varName">  the variable name to remove </param>
		''' <param name="function"> the function to remove the argument from </param>
		Public Overridable Sub removeArgFromOp(ByVal varName As String, ByVal [function] As DifferentialFunction)
			Dim args As val = [function].args()

			For i As Integer = 0 To args.length - 1
				If args(i).name().Equals(varName) Then
					''' <summary>
					''' Since we are removing the variable reference
					''' from the arguments we need to  update both
					''' the reverse and forward arguments.
					''' </summary>
					Dim reverseArgs As IList(Of String) = ops([function].getOwnName()).getInputsToOp()
					Dim newArgs As val = New List(Of String)(args.length - 1)
					For arg As Integer = 0 To args.length - 1
						If Not reverseArgs(arg).Equals(varName) Then
							newArgs.add(reverseArgs(arg))
						End If
					Next arg

					ops([function].getOwnName()).setInputsToOp(newArgs)
					Exit For
				End If
			Next i

			variables(varName).getInputsForOp().remove([function].getOwnName())
		End Sub

		''' <summary>
		''' Get the variable based on the opName
		''' </summary>
		''' <param name="name"> the opName of the variable </param>
		''' <returns> the variabel instance if there is one </returns>
		Public Overridable Function getVariable(ByVal name As String) As SDVariable
			Dim v As Variable = variables(name)
			Return If(v Is Nothing, Nothing, v.getVariable())
		End Function

		Public Overridable Function hasVariable(ByVal name As String) As Boolean
			Return variables_Conflict.ContainsKey(name)
		End Function


		''' <summary>
		''' Get the gradient for the variable with the specified name.<br>
		''' The gradient variable is the variable that represents the derivative of the loss function with respect
		''' to the output of this variable. I.e., if this variable is X and loss function is L, then gradient() returns the
		''' variable representing dL/dX<br>
		''' Note that only floating point variables can have gradients.<br>
		''' Note also that a gradient may not yet be defined, and/or if no loss function variables have been set.<br>
		''' You can set the loss function variables using <seealso cref="SameDiff.setLossVariables(String...)"/> and then create the
		''' gradient functions using <seealso cref="SameDiff.createGradFunction()"/>. Alternatively, the gradient function will be
		''' created automatically when training is performed.
		''' </summary>
		''' <param name="varName"> the vertex id </param>
		''' <returns> the gradient for this variable or null </returns>
		Public Overridable Function getGradForVariable(ByVal varName As String) As SDVariable
			Preconditions.checkState(variables_Conflict.ContainsKey(varName), "No variable with name ""%s"" exists", varName)
			Dim v As SDVariable = getVariable(varName)
			Preconditions.checkState(v.dataType().isFPType(), "Cannot get gradient of %s variable ""%s"": only floating" & " point variables have gradients", varName, v.dataType())
			'Gradients are being placed in the inner "grad" function SameDiff instance, but not the outer one
			If variables_Conflict.ContainsKey(varName) AndAlso variables(varName).getGradient() IsNot Nothing Then
				Return variables(varName).getGradient()
			ElseIf sameDiffFunctionInstances.ContainsKey(GRAD_FN_KEY) AndAlso sameDiffFunctionInstances(GRAD_FN_KEY).variables.ContainsKey(varName) Then
				Return sameDiffFunctionInstances(GRAD_FN_KEY).variables(varName).getGradient()
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Determine if the specified variable has a gradient with respect to the current loss. Note that:
		''' (a) Non-floating-point variables (integer, string, etc) will never have gradients<br>
		''' (b) This method will return false if no gradient function has been created yet. See <seealso cref="SameDiff.createGradFunction()"/>
		''' and <seealso cref="SameDiff.setLossVariables(String...)"/><br>
		''' (c) Floating point variables may not have any gradient if the specified loss variables does not depend on the
		''' specified variable at all. In this case, "no gradient" for floating point is equivalent to "always 0"<br>
		''' </summary>
		''' <param name="varName"> Name of the variable to check the existence of a gradient variable for </param>
		''' <returns> True if a gradient variable exists for the specified variable, for the current loss </returns>
		Public Overridable Function variableHasGradient(ByVal varName As String) As Boolean
			Preconditions.checkState(variables_Conflict.ContainsKey(varName), "No variable with name ""%s"" exists", varName)
			Dim v As SDVariable = getVariable(varName)
			If Not v.dataType().isFPType() OrElse v.Constant Then
				Return False
			End If

			Return getGradForVariable(varName) IsNot Nothing
		End Function


		''' <summary>
		''' Assign a SDVariable to represent the gradient of the SDVariable with the specified name
		''' </summary>
		''' <param name="variableName"> the variable name to assign the gradient variable for </param>
		''' <param name="variable">     the gradient variable </param>
		Public Overridable Sub setGradientForVariableName(ByVal variableName As String, ByVal variable As SDVariable)
			Preconditions.checkState(variables_Conflict.ContainsKey(variableName), "No variable exists with name ""%s""", variableName)
			If variable Is Nothing Then
				Throw New ND4JIllegalStateException("Unable to set null gradient for variable name " & variableName)
			End If
			variables(variableName).setGradient(variable)
		End Sub

		''' <summary>
		''' Get the gradient for the variable with the specified variable name.
		''' Note that in order to run this function, <seealso cref="execBackwards(Map, Operation, MultiDataSet, Collection, List)"/> must be executed first.
		''' All gradient functions are obtained from the results of the execBackwards call.
		''' </summary>
		''' <param name="varName"> the variable name to get the gradient variable for. </param>
		''' <returns> The gradient variable for the specified variable </returns>
		Public Overridable Function grad(ByVal varName As String) As SDVariable
			If Not sameDiffFunctionInstances.ContainsKey(GRAD_FN_KEY) Then
				createGradFunction()
			End If

'JAVA TO VB CONVERTER NOTE: The local variable grad was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim grad_Conflict As SameDiff = getFunction(GRAD_FN_KEY)
			Dim var As SDVariable = grad_Conflict.getVariable(varName)
			Return getFunction(GRAD_FN_KEY).getGradForVariable(var.name())
		End Function


		''' <summary>
		''' Create a new double scalar (rank 0) SDVariable with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the variable with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function scalar(ByVal name As String, ByVal value As Double) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return var(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new float scalar (rank 0) SDVariable with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the variable with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function scalar(ByVal name As String, ByVal value As Single) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return var(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new integer scalar (rank 0) SDVariable with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the variable with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function scalar(ByVal name As String, ByVal value As Integer) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return var(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new long scalar (rank 0) SDVariable with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the variable with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function scalar(ByVal name As String, ByVal value As Long) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return var(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new scalar (rank 0) SDVariable with the specified value and datatype
		''' </summary>
		''' <param name="name">     Name of the SDVariable </param>
		''' <param name="dataType"> Data type of the scalar </param>
		''' <param name="value">    Value to initialize the variable with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function scalar(ByVal name As String, ByVal dataType As DataType, ByVal value As Number) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return var(name, Nd4j.scalar(dataType, value))
			End Using
		End Function

		''' <summary>
		''' Create a new double scalar constant (rank 0) with the specified value.<br>
		''' Constants are not modified by training/backprop. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="value"> Value to initialize the constant with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function constant(ByVal value As Double) As SDVariable
			Return constant(Nothing, value)
		End Function

		''' <summary>
		''' Create a new double scalar constant (rank 0) with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the constant with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function constant(ByVal name As String, ByVal value As Double) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return constant(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new float scalar constant (rank 0) with the specified value<br>
		''' Constants are not modified by training/backprop. See <seealso cref="VariableType"/> for more details.
		''' </summary>
		''' <param name="value"> Value to initialize the constant with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function constant(ByVal value As Single) As SDVariable
			Return constant(Nothing, value)
		End Function

		''' <summary>
		''' Create a new float scalar constant (rank 0) with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the constant with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function constant(ByVal name As String, ByVal value As Single) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return constant(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new integer scalar constant (rank 0) with the specified value
		''' </summary>
		''' <param name="value"> Value to initialize the constant with </param>
		Public Overridable Function constant(ByVal value As Integer) As SDVariable
			Return constant(Nothing, value)
		End Function

		''' <summary>
		''' Create a new integer scalar constant (rank 0) with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the constant with </param>
		''' <returns> SDVariable </returns>
		Public Overridable Function constant(ByVal name As String, ByVal value As Integer) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return constant(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new long scalar constant (rank 0) with the specified value
		''' </summary>
		''' <param name="value"> Value to initialize the constant with </param>
		Public Overridable Function constant(ByVal value As Long) As SDVariable
			Return constant(Nothing, value)
		End Function

		''' <summary>
		''' Create a new long scalar constant (rank 0) with the specified value
		''' </summary>
		''' <param name="name">  Name of the SDVariable </param>
		''' <param name="value"> Value to initialize the constant with </param>
		Public Overridable Function constant(ByVal name As String, ByVal value As Long) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return constant(name, Nd4j.scalar(value))
			End Using
		End Function

		''' <summary>
		''' Create a new scalar constant (rank 0) with the specified value and datatype
		''' </summary>
		''' <param name="name">     Name of the SDVariable </param>
		''' <param name="dataType"> Data type of the scalar constant </param>
		''' <param name="value">    Value to initialize the constant with </param>
		Public Overridable Function constant(ByVal name As String, ByVal dataType As DataType, ByVal value As Number) As SDVariable
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
				Return constant(name, Nd4j.scalar(dataType, value))
			End Using
		End Function

		''' <summary>
		''' Add the specified variable to this SameDiff instance
		''' </summary>
		''' <param name="variable"> Variable to add </param>
		Public Overridable Function addVariable(ByVal variable As SDVariable) As SDVariable
			Preconditions.checkState(variable.getSameDiff() Is Me, "Samediff instance must be the same.")

			If variables_Conflict.ContainsKey(variable.name()) AndAlso Not variables(variable.name()).getVariable().Equals(variable) Then
				Throw New System.ArgumentException("Variable with name """ & variable.name() & """ already exists")
			End If

			Preconditions.checkState(variable.getSameDiff() Is Me, "Same diff instance for variable must be the same!")
			variables(variable.name()) = Variable.builder().name(variable.name()).variable(variable).build()
			Return variable
		End Function


		''' <summary>
		''' Generate the variables based on the given input op and return the output variable names.
		''' </summary>
		''' <param name="function"> the function to generate the output
		'''                 variable names for </param>
		''' <returns> the set of names generated for each output of the function. </returns>
		Public Overridable Function generateOutputVariableForOp(ByVal [function] As DifferentialFunction, ByVal baseName As String, ByVal isImport As Boolean) As SDVariable()
			If baseName Is Nothing Then
				baseName = [function].getOwnName()
			End If

			If baseName Is Nothing Then
				baseName = [function].opName()
			End If

			'First: calculate output data types. We can always calculate output data types, even if the input arrays
			'are not available - *except for sometimes during import, until all ops/variables have been added*
			Dim outputDataTypes As IList(Of DataType) = Nothing

			If Not isImport Then
				Dim inputDataTypes As IList(Of DataType) = New List(Of DataType)()
				Dim fnInputs As IList(Of String) = ops([function].getOwnName()).getInputsToOp()
				If fnInputs IsNot Nothing Then
					For Each var As String In fnInputs
						inputDataTypes.Add(variables(var).getVariable().dataType())
					Next var
				End If
				outputDataTypes = [function].calculateOutputDataTypes(inputDataTypes)
			End If

			'Determine number of output variables
			If TypeOf [function] Is CustomOp Then
				Dim customOp As CustomOp = DirectCast([function], CustomOp)
				Dim num_outputs As Integer = [function].NumOutputs 'Use this in preference - if set. Descriptor might specify 2, but it can sometimes be 2+
				If num_outputs <= 0 Then
					Dim descriptor As val = customOp.Descriptor
					If descriptor IsNot Nothing Then
						num_outputs = descriptor.getNumOutputs()
					End If
					If num_outputs <= 0 Then
						Throw New ND4UnresolvedOutputVariables("Could not determine number of output variables for op " & [function].getOwnName() & " - " & [function].GetType().Name & ". Ops can override" & " getNumOutputs() to specify number of outputs if required")
					End If
				End If
				Dim ret(num_outputs - 1) As SDVariable

				'Infer the output types: we can always determine datatype but not always shapes
				If isImport OrElse (outputDataTypes IsNot Nothing AndAlso outputDataTypes.Count = num_outputs) Then
					log.trace("Incorrect number of output datatypes: got %s but expected datatypes for %s outputs - %s (op: %s), could be due to variable input types.", (If(outputDataTypes Is Nothing, Nothing, outputDataTypes.Count)), num_outputs, outputDataTypes, [function].GetType().Name)
				End If

				'dynamic shapes
				'When importing from TF: convention is "unstack", "unstack:1", "unstack:2", ...
				For i As Integer = 0 To ret.Length - 1
					Dim var As SDVariable = (If(i = 0, getVariable(baseName), getVariable(baseName & ":" & i)))
					If var Is Nothing Then
						'Generate new variable name if one with the specified name doesn't exist
						'Note: output of an op is ARRAY type - activations, not a trainable parameter. Thus has no weight init scheme

						Dim dataType As DataType = If(isImport, Nothing, outputDataTypes(i))
						var = Me.var(generateNewVarName(baseName, i), VariableType.ARRAY, Nothing, dataType, DirectCast(Nothing, Long()))
					End If
					var.setCreator([function])
					ret(i) = var
				Next i

				'Update the internal state: outgoing variables for function
				If getOutputsForOp([function]) Is Nothing Then
					addOutgoingFor(ret, [function])
				End If

				Return ret

			'this is for unresolved shapes, we know xyz is always 1 output
			ElseIf TypeOf [function] Is BaseOp Then
				Dim ret(0) As SDVariable
				Dim checkGet As SDVariable = getVariable(baseName)
				Dim args() As SDVariable = [function].args()
				If checkGet Is Nothing Then
					'Note: output of an op is ARRAY type - activations, not a trainable parameter. Thus has no weight init scheme
					Dim dataType As DataType = outputDataTypes(0)
					checkGet = var(baseName, VariableType.ARRAY, Nothing, dataType, DirectCast(Nothing, Long()))
				End If

				If checkGet Is Nothing Then
					'Note: output of an op is ARRAY type - activations, not a trainable parameter. Thus has no weight init scheme
					Dim dataType As DataType = outputDataTypes(0)
					checkGet = var(baseName, VariableType.ARRAY, Nothing, dataType, DirectCast(Nothing, Long()))
				End If

				checkGet.setCreator([function])
				ret(0) = checkGet


				'Update the internal state: outgoing variables for function
				If getOutputsForOp([function]) Is Nothing Then
					addOutgoingFor(ret, [function])
				End If

				Return ret
			Else
				Throw New Exception("Unknown op type: " & [function].GetType())
			End If
		End Function

		''' <summary>
		''' Generate the variables based on the given input op
		''' and return the output variable names.
		''' </summary>
		''' <param name="function"> the function to generate the output
		'''                 variable names for </param>
		''' <returns> the set of names generated for each output of the function. </returns>
		Public Overridable Function generateOutputVariableForOp(ByVal [function] As DifferentialFunction) As SDVariable()
			Return generateOutputVariableForOp([function],If([function].getOwnName() IsNot Nothing, [function].getOwnName(), [function].opName()), False)
		End Function

		''' <summary>
		''' Get a SameDiff function instance given the name of the function
		''' </summary>
		''' <param name="functionName"> the name of the function </param>
		''' <returns> the same diff function instance defined for the given name </returns>
		Public Overridable Function getFunction(ByVal functionName As String) As SameDiff
			Return sameDiffFunctionInstances(functionName)
		End Function

		''' <summary>
		''' Create a new TensorArray.
		''' </summary>
		Public Overridable Function tensorArray(ByVal dataType As DataType) As TensorArray
			Dim ta As New TensorArray(Me, dataType)
			Dim outVars() As SDVariable = ta.outputVariables()
			Return ta
		End Function

		''' <param name="functionName"> </param>
		''' <param name="with"> </param>
		Public Overridable Function invokeFunctionOn(ByVal functionName As String, ByVal [with] As SameDiff) As SDVariable
			Dim instance As SameDiff = sameDiffFunctionInstances(functionName)
			Dim ret As SDVariable = instance.invokeGraphOn([with])

			Return ret
		End Function


		''' <param name="function"> </param>
		Public Overridable Function defineFunction(ByVal [function] As String, ByVal functionDefinition As SameDiffFunctionDefinition, ByVal variables() As SDVariable) As SameDiff
			If Not sameDiffFunctionInstances.ContainsKey([function]) Then
				Dim [sub] As SameDiff = SameDiff.create()
				Me.child = [sub]
				[sub].parent = Me
				'setup subgraph
				're execute to populate subgraph
				Dim ret(variables.Length - 1) As SDVariable
				For i As Integer = 0 To ret.Length - 1
					ret(i) = [sub].var(variables(i))
				Next i

				functionDefinition.define([sub], Nothing, ret)
				sameDiffFunctionInstances([function]) = [sub]
			End If
			Me.child = Nothing
			Return sameDiffFunctionInstances([function])
		End Function


		''' <param name="function"> </param>
		Public Overridable Sub defineFunction(ByVal [function] As String, ByVal functionDefinition As SameDiffFunctionDefinition)
			defineFunction([function], functionDefinition, New LinkedHashMap(Of String, INDArray)())
		End Sub

		''' <param name="function"> </param>
		''' <param name="functionDefinition"> </param>
		''' <param name="inputs"> </param>
		Public Overridable Sub defineFunction(ByVal [function] As String, ByVal functionDefinition As SameDiffFunctionDefinition, ByVal inputs As IDictionary(Of String, INDArray))
			If Not sameDiffFunctionInstances.ContainsKey([function]) Then
				Dim [sub] As SameDiff = SameDiff.create()
				'setup subgraph
				're execute to populate subgraph
				functionDefinition.define([sub], inputs, Nothing)

				sameDiffFunctionInstances([function]) = [sub]
			End If
		End Sub

		''' <summary>
		''' See <seealso cref="calculateGradients(Map, Collection)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> calculateGradients(Map<String, org.nd4j.linalg.api.ndarray.INDArray> placeholderVals, @NonNull String... variables)
		Public Overridable Function calculateGradients(ByVal placeholderVals As IDictionary(Of String, INDArray), ParamArray ByVal variables() As String) As IDictionary(Of String, INDArray)
			Preconditions.checkArgument(variables.Length > 0, "No variables were specified")
			Return calculateGradients(placeholderVals, java.util.Arrays.asList(variables))
		End Function

		''' <summary>
		''' Calculate and return the gradients for the specified variables
		''' </summary>
		''' <param name="placeholderVals"> Placeholders. May be null </param>
		''' <param name="variables">       Names of the variables that you want the gradient arrays for </param>
		''' <returns> Gradients as a map, keyed by the variable name </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Map<String, org.nd4j.linalg.api.ndarray.INDArray> calculateGradients(Map<String, org.nd4j.linalg.api.ndarray.INDArray> placeholderVals, @NonNull Collection<String> variables)
		Public Overridable Function calculateGradients(ByVal placeholderVals As IDictionary(Of String, INDArray), ByVal variables As ICollection(Of String)) As IDictionary(Of String, INDArray)
			Preconditions.checkArgument(variables.Count > 0, "No variables were specified")
			Dim oag As OutAndGrad = calculateGradientsAndOutputs(placeholderVals, Nothing, variables)
			Return oag.getGradients()
		End Function

		''' <summary>
		''' Calculate the activations and the gradients for the specified variables, in one execution call.
		''' This is equivalent to calling <seealso cref="output(Map, List)"/> and <seealso cref="calculateGradients(Map, Collection)"/>, but
		''' is more efficient than calling both separately.
		''' </summary>
		''' <param name="placeholderVals"> Placeholders. May be null </param>
		''' <param name="outputVars">      Names of the variables that you want the activations/outputs for. May be null </param>
		''' <param name="gradientVars">    Names of the variables that you want the gradient arrays for. May be null </param>
		''' <returns> Activations and gradients, keyed by variable name </returns>
		Public Overridable Function calculateGradientsAndOutputs(ByVal placeholderVals As IDictionary(Of String, INDArray), ByVal outputVars As ICollection(Of String), ByVal gradientVars As ICollection(Of String)) As OutAndGrad
			Preconditions.checkArgument((outputVars IsNot Nothing AndAlso outputVars.Count > 0) OrElse (gradientVars IsNot Nothing AndAlso gradientVars.Count > 0), "No variables were specified for either output or gradients")
			If getFunction(GRAD_FN_KEY) Is Nothing Then
				createGradFunction()
			End If

			Dim varNames As IList(Of String) = New List(Of String)()
			If outputVars IsNot Nothing Then
				CType(varNames, List(Of String)).AddRange(outputVars)
			End If
			If gradientVars IsNot Nothing Then
				For Each s As String In gradientVars
					Preconditions.checkState(Me.variables_Conflict.ContainsKey(s), "No variable with name ""%s"" exists in the SameDiff instance", s)
					Dim v As SDVariable = getVariable(s).Gradient
					If v IsNot Nothing Then
						'In a few cases (like loss not depending on trainable parameters) we won't have gradient array for parameter variable
						varNames.Add(v.name())
					End If
				Next s
			End If

			'Key is gradient variable name
			Dim gradFn As SameDiff = getFunction(GRAD_FN_KEY)
			gradFn.setListeners(listeners_Conflict)
			Dim grads As IDictionary(Of String, INDArray) = gradFn.batchOutputHelper(placeholderVals, Nothing, Operation.TRAINING, CType(varNames, List(Of String)).ToArray())

			Dim outOutputs As IDictionary(Of String, INDArray) = If(outputVars Is Nothing, Nothing, New Dictionary(Of String, INDArray)())
			Dim outGrads As IDictionary(Of String, INDArray) = If(gradientVars Is Nothing, Nothing, New Dictionary(Of String, INDArray)())
			If outputVars IsNot Nothing Then
				For Each s As String In outputVars
					outOutputs(s) = grads(s)
				Next s
			End If
			If gradientVars IsNot Nothing Then
				For Each s As String In gradientVars
					If getVariable(s).Gradient IsNot Nothing Then
						Dim gradVar As String = getVariable(s).Gradient.name()
						outGrads(s) = grads(gradVar)
					End If
				Next s
			End If

			Return New OutAndGrad(outOutputs, outGrads)
		End Function

		''' <summary>
		''' Returns true if the gradient function has been created - i.e., <seealso cref="createGradFunction()"/> or <seealso cref="createGradFunction(String...)"/>
		''' has been called at all
		''' </summary>
		''' <returns> True if gradient (backprop) function exists </returns>
		Public Overridable Function hasGradientFunction() As Boolean
			Return sameDiffFunctionInstances.ContainsKey(GRAD_FN_KEY)
		End Function

		''' <summary>
		''' Create the gradient function (for calculating gradients via <seealso cref="calculateGradients(Map, Collection)"/>) if it is not already defined.
		''' Users do not usually need to call this function manually, as it is called as required in the aforementioned method.
		''' <br><br>
		''' If the gradient function already exists, this method is a no-op.<br>
		''' After this method returns, the SameDiff function instance for the gradient can be accessed using <seealso cref="getFunction(String)"/>
		''' with name "grad" as the argument.<br>
		''' Note that the gradient array (after execBackwards has been called) can be accessed via {@code SDVariable.gradient().getArr()}
		''' </summary>
		Public Overridable Sub createGradFunction()
			createGradFunction(DirectCast(Nothing, String()))
		End Sub

		''' <summary>
		''' As per <seealso cref="createGradFunction()"/>, but this method allows a set of variables requiring gradients to be specified.
		''' By default, only parameter gradients will be calculated; placeholder gradients may not be defined (unless they happen
		''' to be calculated in the same op as calculating a parameter gradient.
		''' This method allows you to override this behaviour by passing the name of the placeholder you want the gradients for.
		''' The specified gradient variables still need to be floating point variables.
		''' </summary>
		''' <param name="variablesRequiringGradients"> May be null. If non-null: the gradients for the variables with these names will
		'''                                    be calculated and available after backprop has been done </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public void createGradFunction(final String... variablesRequiringGradients)
		Public Overridable Sub createGradFunction(ParamArray ByVal variablesRequiringGradients() As String)
			If lossVariables_Conflict.Count = 0 Then
				If trainingConfig_Conflict IsNot Nothing AndAlso trainingConfig_Conflict.getLossVariables() IsNot Nothing AndAlso Not trainingConfig_Conflict.getLossVariables().isEmpty() Then
					CType(lossVariables_Conflict, List(Of String)).AddRange(trainingConfig_Conflict.getLossVariables())
				Else
					Dim lossInferred As IList(Of String) = bestGuessLossVariables()
					If lossInferred.Count = 1 Then
						Dim outName As String = lossInferred(0)
						Dim opName As String = variables(outName).getOutputOfOp()
						If opName Is Nothing OrElse Not (TypeOf ops(opName).getOp() Is ExternalErrorsFunction) Then
							log.info("Inferring output ""{}"" as loss variable as none were previously set." & "Use SameDiff.setLossVariables() or SDVariable.markAsLoss() to override", lossInferred(0))
						End If
						lossVariables_Conflict.Add(lossInferred(0))
					ElseIf lossInferred.Count = 0 Then
						'Check for external errors function
						For Each o As SameDiffOp In ops_Conflict.Values
							If TypeOf o.Op Is ExternalErrorsFunction Then
								Dim l As IList(Of String) = o.getOutputsOfOp()
								lossVariables_Conflict.Add(l(0))
							End If
						Next o
					End If
				End If
			End If

			Preconditions.checkState(lossVariables_Conflict.Count > 0, "Cannot create gradient function: " & "No loss variables (variables to minimize) have been specified. Loss variables are the variables that" & " represent the loss/cost/score to be minimized during training, and that all gradients are calculated with respect to." & vbLf & " Losses can be specified either in TrainingConfiguration (Builder.minimize(...)) or via SameDiff.setLossVariables()/addLossVariable()")

			If log.isTraceEnabled() Then
				log.trace("Defining function ""grad""")
			End If

			If variablesRequiringGradients IsNot Nothing AndAlso variablesRequiringGradients.Length > 0 Then
				'Check that they are FP variables...
				For Each s As String In variablesRequiringGradients
					Preconditions.checkArgument(variables_Conflict.ContainsKey(s), "Cannot ensure gradient exists for variable: no variable with name ""%s"" exists", s)
					Dim dt As DataType = variables(s).getVariable().dataType()
					Preconditions.checkState(dt.isFPType(), "Cannot ensure gradient exists for variable ""%s"": variable is not a floating point SDVariable." & " Only floating point SDVariables have gradients defined - variable has type %s", s, dt)
				Next s
			End If


	'        
	'        Defining gradient function:
	'
	'        Starting point:
	'        (a) Set of loss function variables - i.e., one or more variables representing loss to be minimized
	'        (b) Set of floating point variables we want to train (Variable type SDVariables only - not constants, arrays, placeholders)
	'
	'        Observation: A trainable parameter only has a gradient defined if there is a floating point path between the variable and the loss.
	'        for example: X(fp) -> cast(int) -> cast(fp) -> loss - X has no gradient defined
	'
	'        Algorithm for backprop:
	'
	'        Step 1: Determine if variable requires a gradient (is trainable)
	'        How? Walk backward on op graph starting at loss variable(s), along FP variables only.
	'        Collect FP variables in set as we go.
	'        This gives us a subgraph "connected to loss by FP path" - gradient for FP variable is defined only if it's in that set/subgraph.
	'
	'
	'        Step 2: Determine minimal set of variables (including array type SDVariables - i.e., activations) we need gradients for
	'        Consider following graph: X(fp) -> cast(int) -> cast(fp) -> lots of FP ops -> loss
	'        unless we need them for other variables, there's zero point calculating the activation gradients for the "cast(fp) -> lots of FP ops" part of the graph, as the gradient from that branch won't go anywhere.
	'        How to determine minimal subset? Start with FP graph from step 1... then keep pruning leaves until the only remaining leaves are those FP variables that we need gradients for.
	'        Note that the user can also specify variables that they need gradients for (like placeholders) that normally wouldn't get gradients.
	'
	'        Step 3: Differentiate ops in minimal subgraph
	'        The only major issue here is with multiple output ops, where only one of the outputs lead to the loss.
	'        For example, X -> slice -> (A,B); B -> loss, with A being unused (in that it doesn't contribute to the loss function)
	'        But to do split op backprop, we need gradient variables/arrays for both outputs (A and B).
	'        We know the shape and type of dL/dA must be exactly the same as A; we also know that the loss function doesn't depend on A. Hence, dL/dA == zerosLike(A)
	'
	'         


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SameDiff outer = this;
			Dim outer As SameDiff = Me
			defineFunction(GRAD_FN_KEY, New SameDiffFunctionDefinitionAnonymousInnerClass(Me, variablesRequiringGradients, outer))

			associateSameDiffWithOpsAndVariables()
		End Sub

		Private Class SameDiffFunctionDefinitionAnonymousInnerClass
			Implements SameDiffFunctionDefinition

			Private ReadOnly outerInstance As SameDiff

			private String() variablesRequiringGradients
			private org.nd4j.autodiff.samediff.SameDiff outer

			public SameDiffFunctionDefinitionAnonymousInnerClass(SameDiff outerInstance, String() variablesRequiringGradients, org.nd4j.autodiff.samediff.SameDiff outer)
			If True Then
				Me.outerInstance = outerInstance
				Me.variablesRequiringGradients = variablesRequiringGradients
				Me.outer = outer
			End If


			public SDVariable() define(SameDiff sameDiff, IDictionary(Of String, INDArray) inputs, SDVariable() variableInputs)
			If True Then
				sameDiff.setArrayHolders(New SingleThreadArrayHolder(), New SingleThreadArrayHolder(), False) 'Training isn't thread safe, no need to use DeviceLocal, even with lazy init

				'Propagate graph to this samediff instance which will also contain the backward
				If outerInstance.debugMode Then
					sameDiff.enableDebugMode()
				End If

				outer.invokeGraphOn(sameDiff)
				If outerInstance.debugMode Then
					'Expect incoming args and outgoing args to be the same
					Preconditions.checkState(sameDiff.ops.keySet().Equals(outerInstance.ops_Conflict.Keys), "ops keysets not equal")
				End If

				Dim allFunctions As IList(Of SameDiffOp) = New List(Of SameDiffOp)(sameDiff.ops.values())
				If allFunctions.Count = 0 Then
					Throw New ND4JIllegalStateException("No ops found!")
				End If

				For Each op As SameDiffOp In allFunctions
					Dim func As DifferentialFunction = op.Op

					Dim args As val = func.args()
					For Each arg As val In args
						arg.setSameDiff(sameDiff)
					Next arg
					Dim outputs As val = func.outputVariables()
					For Each output As val In outputs
						output.setSameDiff(sameDiff)
					Next output
					func.setSameDiff(sameDiff)
				Next op

				Dim finalOutputs As IList(Of SDVariable) = New List(Of SDVariable)(outerInstance.lossVariables_Conflict.Count)
				Dim initialGrad As SDVariable = sameDiff.var("one-var", Nd4j.scalar(1.0f))
				For Each s As String In outerInstance.lossVariables_Conflict
					Preconditions.checkNotNull(s, "Encountered null value in loss variables. Null loss variables are not allowed." & " Use SameDiff.setLossVariables with non-null array names to fix")
					Preconditions.checkState(outerInstance.variables_Conflict.ContainsKey(s), "Specified loss function variable ""%s"" does not exist", s)
					Dim v As SDVariable = outerInstance.variables_Conflict(s).getVariable()
					Preconditions.checkState(v.dataType().isFPType(), "Specified loss function variable ""%s"" is not a floating" & "point variable (datatype: %s). Only floating point variables may be used as loss function variable", s, v.dataType())
					v = v.sum() 'If output is not a scalar: we'll use loss = v.sum(), same as adding loss for multiple outputs. We don't always know for sure if output is scalar at this point
					If v.dataType() = initialGrad.dataType() Then
						sameDiff.setGradientForVariableName(v.name(), initialGrad)
					Else
						sameDiff.setGradientForVariableName(v.name(), initialGrad.castTo(v.dataType()))
					End If
					If finalOutputs.Contains(v) Then
						log.warn("Loss function variable ""{}"" appears multiple times in list of loss variables - using only first instance", s)
					Else
						finalOutputs.Add(v)
					End If
				Next s

				If log.isTraceEnabled() Then
					Dim initialOutputsStr() As String = allFunctions(allFunctions.Count - 1).getOp().outputVariablesNames()
					Dim s As String = If(initialOutputsStr Is Nothing, "null", java.util.Arrays.toString(initialOutputsStr))
					log.trace("Defining backward function: initial outputs {}", s)
				End If


				'----- Step 1: Determine FP variables connected to loss -----
				' Find all FP variables that are connected to loss by an floating point (FP16/32/64) path
				Dim allFpVarsConnectedToLoss As ISet(Of String) = New HashSet(Of String)()
				Dim toProcess As New LinkedList(Of String)()
				For Each s As String In outerInstance.lossVariables_Conflict
					If Not toProcess.Contains(s) Then
						toProcess.AddLast(s)
					End If
				Next s
				Do While toProcess.Count > 0
					Dim [next] As String = toProcess.RemoveFirst()
					If Not allFpVarsConnectedToLoss.Contains([next]) Then
						Dim v As Variable = outerInstance.variables_Conflict([next])
						If v.getVariable().dataType().isFPType() Then
							allFpVarsConnectedToLoss.Add(v.getName())
							'Work out what op (if any) this is an output of... and add the inputs to that op to be processed
							If v.getOutputOfOp() IsNot Nothing Then
								Dim opName As String = v.getOutputOfOp()
								Dim op As SameDiffOp = outerInstance.ops_Conflict(opName)
								Dim opInputs As IList(Of String) = op.getInputsToOp()
								If opInputs IsNot Nothing Then
									For Each s As String In opInputs
										Dim inputVar As Variable = outerInstance.variables_Conflict(s)
										If inputVar.getVariable().dataType().isFPType() Then
											'Add this connected floating point type to the list to be processed
											toProcess.AddLast(s)
										End If
									Next s
								End If
							End If
						End If
					End If
				Loop

				'----- Step 2: Determine minimal set of FP variables actually required -----
				' Keep removing leaf nodes until only Variable type SDVariables remain
				Dim minimalSubgraphVars As ISet(Of String) = New HashSet(Of String)(allFpVarsConnectedToLoss)
				Dim leafFPVars As New LinkedList(Of String)()
				For Each s As String In allFpVarsConnectedToLoss
					'First: determine if is a FP leaf (Array type SDVariable)
					Dim v As Variable = outerInstance.variables_Conflict(s)
					If v.getVariable().getVariableType() = VariableType.ARRAY Then
						Dim opName As String = v.getOutputOfOp() 'Always defined for array type
						Dim op As SameDiffOp = outerInstance.ops_Conflict(opName)
						Dim inputsToOp As IList(Of String) = op.getInputsToOp()
						Dim anyInputsInSubgraph As Boolean = False
						If inputsToOp IsNot Nothing Then
							For Each s2 As String In inputsToOp
								If allFpVarsConnectedToLoss.Contains(s2) Then
									'Connection s2 -> s exists... therefore s is not a leaf (yet)
									anyInputsInSubgraph = True
									Exit For
								End If
							Next s2
						End If
						If Not anyInputsInSubgraph Then
							'Mark s as a leaf to be removed
							leafFPVars.AddLast(s)
						End If
					End If
					Dim vt As VariableType = v.getVariable().getVariableType()
					Dim isUserRequested As Boolean = variablesRequiringGradients IsNot Nothing AndAlso ArrayUtils.contains(variablesRequiringGradients, s)
					If (vt = VariableType.CONSTANT OrElse vt = VariableType.PLACEHOLDER) AndAlso Not isUserRequested Then
						leafFPVars.AddLast(s)
					End If
				Next s

				Do While leafFPVars.Count > 0
					Dim nextLeaf As String = leafFPVars.RemoveFirst()
					Dim v As Variable = outerInstance.variables_Conflict(nextLeaf)
					minimalSubgraphVars.remove(nextLeaf)

					'Now, after removing: check what this variable is input to...
					'If nextLeaf is input to some op X, then if none of inputs y->X are present in subgraph, then
					' output variables X->z must now be leafs
					'Note that any time we remove a variable, the only possible new leafs are those that this one
					' is connected to.
					Dim inputsTo As IList(Of String) = v.getInputsForOp()
					If inputsTo IsNot Nothing AndAlso inputsTo.Count > 0 Then
						For Each opName As String In inputsTo
							Dim op As SameDiffOp = outerInstance.ops_Conflict(opName)
							Dim inputsToOp As IList(Of String) = op.getInputsToOp()
							Dim anyPresent As Boolean = False
							For Each s As String In inputsToOp
								If minimalSubgraphVars.Contains(s) OrElse (variablesRequiringGradients IsNot Nothing AndAlso ArrayUtils.contains(variablesRequiringGradients, s)) Then
									'Note second condition: means user explicitly specified that they want gradients for that input variable... hence we need to diff this op
									anyPresent = True
									Exit For
								End If
							Next s
							If Not anyPresent Then
								'All inputs to op X are not in subgraph. Therefore outputs of op must be new leaves
								Dim outVars As IList(Of String) = op.getOutputsOfOp()
								If outVars IsNot Nothing Then
									For Each s As String In outVars
										If Not leafFPVars.Contains(s) Then
											'Mark this variable to be processed next
											leafFPVars.AddLast(s)
										End If
									Next s
								End If
							End If
						Next opName
					End If
				Loop

				Preconditions.checkState(minimalSubgraphVars.Count > 0, "Cannot differentiate graph relative to the specified loss function variables %s:" & " graph does not contain any trainable SDVariables (floating point VARIABLE type SDVariables) that the loss function depend on.", outerInstance.lossVariables_Conflict)

				'At this point: we know the set of variables that are connected to the loss - these all (and only) need gradients
				Dim availableForDiff As New LinkedList(Of String)()
				For Each lossVar As SDVariable In finalOutputs
					Dim v As Variable = sameDiff.variables.get(lossVar.name())
					If v.getOutputOfOp() IsNot Nothing Then
						Dim opName As String = v.getOutputOfOp()
						availableForDiff.AddLast(opName)
					End If
				Next lossVar

				' Collect all the the ops that have to be traversed before we can conclude that the gradient for
				' a variable is fully available
				'For example, if we have  X -> op -> Y, and Y -> (A,B) we need gradient contribution from BOTH
				' Y->A and Y->B connections before we can do differentiation of op "op"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final HashMap<String, List<String>> prerequisites = new HashMap<>();
				Dim prerequisites As New Dictionary(Of String, IList(Of String))() 'Key: variable name. Value: list of op names
				For Each var As String In minimalSubgraphVars
					Dim variable As Variable = outerInstance.variables_Conflict(var)
					' Copy the collection, as the original one will be modified during backprop
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<String> inputsForOp = variable.getInputsForOp();
					Dim inputsForOp As IList(Of String) = variable.getInputsForOp()
					If inputsForOp IsNot Nothing Then
						Dim req As IList(Of String) = New List(Of String)()
						For Each opName As String In inputsForOp
							'Need to filter ops here
							'For example, if we have: var -> Op1, and var -> Op2
							'we might not need to differentiate Op2 if output of Op2 doesn't impact loss function
							Dim o As SameDiffOp = outerInstance.ops_Conflict(opName)
							Dim opOutputs As IList(Of String) = o.getOutputsOfOp()
							Dim anyOpOutputsRequired As Boolean = False
							If opOutputs IsNot Nothing Then
								For Each s As String In opOutputs
									If minimalSubgraphVars.Contains(s) Then
										anyOpOutputsRequired = True
										Exit For
									End If
								Next s
							End If
							If anyOpOutputsRequired Then
								req.Add(opName)
							End If
						Next opName
						prerequisites(variable.getName()) = req
					End If
				Next var

				Dim differentiatedOps As ISet(Of String) = New HashSet(Of String)()
				Do While availableForDiff.Count > 0
					Dim dfName As String = availableForDiff.RemoveFirst()
					Dim df As DifferentialFunction = sameDiff.ops.get(dfName).getOp()

					'Get the inputs and outputs of the op
					Dim inputsToOp As IList(Of String)
					Dim outputsOfOp As IList(Of String)
					If TypeOf df Is GradientBackwardsMarker Then
						Dim op As SameDiffOp = sameDiff.ops.get(df.getOwnName())
						inputsToOp = op.getInputsToOp()
						outputsOfOp = java.util.Collections.emptyList()
					Else
						inputsToOp = sameDiff.ops.get(df.getOwnName()).getInputsToOp()
						outputsOfOp = sameDiff.ops.get(df.getOwnName()).getOutputsOfOp()
					End If


					'Get gradients for all output variables:
					Dim grads As IList(Of SDVariable) = New List(Of SDVariable)()
					For Each s As String In outputsOfOp
						Dim v As SDVariable = sameDiff.getVariable(s)
						Dim g As SDVariable = If(v.hasGradient(), v.gradient(), Nothing)

						If g Is Nothing Then
							'If no gradient exists at this point, 3 possibilities:
							' (a) we have a bug
							' (b) output of this op isn't used in calculating the loss
							' (c) output isn't a FP type
							'In the FP case, we should create a zero variable to backprop, because we can't perform backprop
							' for this op otherwise...
							If Not v.dataType().isFPType() Then
								grads.Add(Nothing)
							Else
								'See "Step 3: Differentiate ops in minimal subgraph" above for explanation on why this should be zerosLike here...
								Dim gTemp As SDVariable = sameDiff.zerosLike(v)
								grads.Add(gTemp)
							End If
						Else
							grads.Add(g)
						End If
					Next s

					'Differentiate:
					Dim currFnGrads As IList(Of SDVariable) = df.diff(grads)
					differentiatedOps.Add(df.getOwnName())

					'Check the inputs to this op, see if we can differentiate those ops now (and if so: add to queue)
					For Each s As String In inputsToOp
						Dim v As Variable = sameDiff.variables.get(s)
						Dim opName As String = v.getOutputOfOp()
						If opName Is Nothing OrElse differentiatedOps.Contains(opName) Then
							'Skip placeholder/constant etc; also skip if we've previously differentiated this op
							Continue For
						End If

						'Next: we've just differentiated OpX
						'For s -> OpX: we now have gradient for s after df.diff(grads) call earlier
						'Now, do we also need to differentiate OpY, where OpY -> s?
						'If any input variables x (x -> OpY) exist, if they are in the minimal subgraph, then we
						' need to differentiate OpY too
						'Note that just because we *need to* doesn't mean we *can* yet

						Dim isRequiredOp As Boolean = False
						Dim op As SameDiffOp = outerInstance.ops_Conflict(opName)
						If op.getInputsToOp() IsNot Nothing Then
							Dim opInputs As IList(Of String) = op.getInputsToOp()
							Dim anyInputsRequired As Boolean = False
							For Each s2 As String In opInputs
								If minimalSubgraphVars.Contains(s2) Then
									anyInputsRequired = True
									Exit For
								End If
							Next s2
							If anyInputsRequired Then
								If Not differentiatedOps.Contains(op.Name) Then
									isRequiredOp = True
								End If
							End If
						End If

						If Not isRequiredOp Then
							Continue For
						End If

						'Now that we know we need this op - check if we can actually differentiate it...
						'We can differentiate it if, for all variables that are outputs of this op:
						'(a) we have gradient already, OR
						'(b) it's not a FP variable, OR
						'(c) it's a FP variable but not one that the loss depends on
						'Note that for "output array is used multiple times" case (i.e., X->opY->Y, X->opZ->Z) we need all gradient
						' contributions - i.e., we need to have differentiated both opY and opZ

						Dim allAvailable As Boolean = True
						Dim o As SameDiffOp = sameDiff.ops.get(opName)
						For Each opOutput As String In o.getOutputsOfOp()
							Dim outVar As Variable = outerInstance.variables_Conflict(opOutput)
							If outVar.getVariable().dataType().isFPType() Then
								If minimalSubgraphVars.Contains(outVar.getName()) Then
									'Need gradient for this variable to be available before we can differentiate
									If outVar.getVariable().gradient() Is Nothing Then
										allAvailable = False
										Exit For
									End If
									'However, when a variable is used multiple times, we need ALL gradient contributions available:
									Dim prereqs As IList(Of String) = prerequisites(outVar.getName())
									If prereqs IsNot Nothing Then
										allAvailable = allAvailable And differentiatedOps.ContainsAll(prereqs)
										If Not allAvailable Then
											Exit For
										End If
									End If
								End If
								'If in't not in the minimal subgraph, loss doesn't depend on it, so we don't care about it
							End If
						Next opOutput

						If allAvailable AndAlso Not availableForDiff.Contains(o.Op.getOwnName()) Then
							availableForDiff.AddLast(o.Op.getOwnName())
						End If
					Next s
				Loop

				'Let's validate we actually differentiated everything correctly:
				For Each s As String In minimalSubgraphVars
					If outerInstance.lossVariables_Conflict.Contains(s) Then
						Continue For
					End If
					Dim v As SDVariable = outerInstance.variables_Conflict(s).getVariable()
					Dim g As SDVariable = v.gradient()
					If g Is Nothing Then
						Throw New System.InvalidOperationException("Error encountered during differentiation: no gradient for required variable """ & s & """ was calculated")
					End If
				Next s


				Return New SDVariable(){sameDiff.var(GRAD_FN_KEY, DataType.FLOAT, 1)}
			End If
		End Class


		''' <summary>
		''' Try to infer the loss variable/s (usually loss variables). Note that this is not reliable in general.
		''' </summary>
		protected IList(Of String) bestGuessLossVariables()
		If True Then
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each v As Variable In variables_Conflict.Values
				If v.getVariable().isConstant() OrElse v.getVariable().isPlaceHolder() OrElse (v.getInputsForOp() IsNot Nothing AndAlso Not v.getInputsForOp().isEmpty()) OrElse (v.getControlDepsForOp() IsNot Nothing AndAlso Not v.getControlDepsForOp().isEmpty()) OrElse (v.getControlDepsForVar() IsNot Nothing AndAlso Not v.getControlDepsForVar().isEmpty()) Then 'Exclude variables that are control dependency inputs to other variables (mainly for import of cond etc ops)
					Continue For
				End If

				'Also exclude assert etc ops - doesn't make sense to return these "outputs" to user
				If v.getOutputOfOp() IsNot Nothing AndAlso v.getVariable().dataType().isFPType() Then
					Dim opName As String = v.getOutputOfOp()
					Dim o As SameDiffOp = ops(opName)
					If TypeOf o.Op Is Assert Then
						Continue For
					End If

					'A bit of a hack for TF import: some TF graphs have Switch ops, where the output of one branch isn't consumed
					' by any ops. Consequently, during execution this "output" might never be available. So we'll exclude the output of execution here
					' This applies to SameDiff while loops as well
					If TypeOf o.Op Is Switch Then
						Continue For
					End If
				End If


				[out].Add(v.getName())
			Next v
			Return [out]
		End If

		''' <summary>
		''' Returns true if this vertex id is a place holder variable or not<br>
		''' A place holder variable is one where the array shape(s) are currently known and can't yet be calculated
		''' </summary>
		''' <param name="varName"> the vertex id to test </param>
		''' <returns> True if the variable is a placeholder, false otherwise </returns>
		public Boolean isPlaceHolder(String varName)
		If True Then
			Preconditions.checkState(variables_Conflict.ContainsKey(varName), "No variable present in SameDiff instance with name ""%s""", varName)
			Return variables(varName).getVariable().isPlaceHolder()
		End If


		''' <summary>
		''' Updates the variable name property on the passed in variable, the reference in samediff, and returns the variable.
		''' <para>
		''' Note that if null for the new variable is passed in, it will just return the original input variable.
		''' </para>
		''' </summary>
		''' <param name="opToRename">  note we pass in the op here for times when an op may have multiple outputs
		'''                    when this is the case, we need to pass in the op to rename otherwise context gets lost
		'''                    and subsequent rename attempts will not operate on the op. </param>
		''' <param name="varToUpdate"> the variable to update </param>
		''' <param name="newVarName">  the new variable name </param>
		''' <returns> the passed in variable </returns>
		public SDVariable updateVariableNameAndReference(SameDiffOp opToRename,SDVariable varToUpdate, String newVarName)
		If True Then
			If varToUpdate Is Nothing Then
				Throw New System.NullReferenceException("Null input: No variable found for updating!")
			End If

			If newVarName IsNot Nothing Then
				Dim nameScope As String = currentNameScope()
				If nameScope IsNot Nothing Then
					If Not newVarName.StartsWith(nameScope & "/") Then
						newVarName = nameScope & "/" & newVarName
					End If
				End If
			End If

			If newVarName IsNot Nothing AndAlso variables_Conflict.ContainsKey(newVarName) AndAlso varToUpdate <> variables(newVarName).getVariable() Then
				Throw New System.InvalidOperationException("Variable name """ & newVarName & """ already exists for a different SDVariable")
			End If

			If newVarName Is Nothing AndAlso variables_Conflict.ContainsKey(varToUpdate.name()) AndAlso variables(varToUpdate.name()).getVariable() <> varToUpdate Then
				'Edge case: suppose we do m1=sd.mean(in), m2=sd.mean(m1) -> both initially have the name
				' "mean" and consequently a new variable name needs to be generated
				newVarName = generateNewVarName(varToUpdate.name(), 0)
			End If

			If newVarName Is Nothing OrElse varToUpdate.name().Equals(newVarName) Then
				Return varToUpdate
			End If

			Dim oldVarName As val = varToUpdate.name()
			varToUpdate.setVarName(newVarName)
			renameVariable(opToRename,oldVarName, newVarName)
			Return varToUpdate
		End If

		''' <summary>
		''' Updates the variable name property on the passed in variable, the reference in samediff, and returns the variable.
		''' <para>
		''' Note that if null for the new variable is passed in, it will just return the original input variable.
		''' 
		''' </para>
		''' </summary>
		''' <param name="varToUpdate"> the variable to update </param>
		''' <param name="newVarName">  the new variable name </param>
		''' <returns> the passed in variable </returns>
		public SDVariable updateVariableNameAndReference(SDVariable varToUpdate, String newVarName)
		If True Then
			Dim op As SameDiffOp = ops(varToUpdate.name())
			Return updateVariableNameAndReference(op,varToUpdate,newVarName)
		End If


		''' <summary>
		''' Updates the variable name property on the passed in variables, its reference in samediff, and returns the variable.
		''' </summary>
		''' <param name="variablesToUpdate"> the variable to update </param>
		''' <param name="newVariableNames">  the new variable name </param>
		''' <returns> the updated, passed in variables </returns>
		public SDVariable() updateVariableNamesAndReferences(SDVariable() variablesToUpdate, String() newVariableNames)
		If True Then

			Dim numVariables As Integer = variablesToUpdate.length
			Dim updatedVariables(numVariables - 1) As SDVariable

			For i As Integer = 0 To numVariables - 1
				Dim varToUpdate As SDVariable = variablesToUpdate(i)
				Dim name As String = If(newVariableNames Is Nothing, Nothing, newVariableNames(i))
				updatedVariables(i) = updateVariableNameAndReference(varToUpdate, name)
			Next i

			Return updatedVariables
		End If

		''' <summary>
		''' Associate the current SameDiff instance with all ops and variables.
		''' This is necessary to ensure that when dealing with shared state (usually with a SameDiff function such
		''' as "grad" - the backward function) we have the correct SameDiff instance set for all ops/SDVariables.<br>
		''' If this is not done, arrays and shapes could be fetched from the incorrect SameDiff instance for some methods
		''' </summary>
		protected void associateSameDiffWithOpsAndVariables()
		If True Then
			For Each var As SDVariable In variableMap().Values
				var.setSameDiff(Me)
			Next var
	'        for(DifferentialFunction df : functionInstancesById.values()){
			For Each op As SameDiffOp In ops_Conflict.Values
				Dim df As DifferentialFunction = op.Op
				df.setSameDiff(Me)

				'TODO: This is ugly but seemingly necessary
				'Finally, also set the SDVariable for each op
				'Otherwise: could have an op pointing to this SameDiff instance, but op's SDVariable's sameDiff field pointing
				' to another SameDiff instance. At which point, they could fetch shapes and arrays from some other instance
				' (i.e., not from this one that is currently executing)
				Dim args() As SDVariable = df.args()
				If args IsNot Nothing Then
					For Each arg As SDVariable In args
						arg.setSameDiff(Me)
					Next arg
				End If

				Dim outputs() As SDVariable = df.outputVariables()
				If outputs IsNot Nothing Then
					For Each [out] As SDVariable In outputs
						[out].setSameDiff(Me)
					Next [out]
				End If
			Next op
		End If


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected int asFlatNode(String name, @NonNull SameDiff scope, @NonNull FlatBufferBuilder bufferBuilder)
		protected Integer asFlatNode(String name, SameDiff scope, FlatBufferBuilder bufferBuilder)
		If True Then
			Dim scopeName As Integer = bufferBuilder.createString(name)

'JAVA TO VB CONVERTER NOTE: The variable flatNode was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim flatNode_Conflict As Integer = FlatNode.createFlatNode(bufferBuilder, scopeName, scopeName, OpType.LOGIC, 10, 0, 0, 0, CSByte(0), 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0)

			Return flatNode_Conflict
		End If

		''' <summary>
		''' Note: INTENDED FOR DEVELOPER USE<br>
		''' This method extract base variable name and output index (if exists) from raw variable name.
		''' I.e:
		''' - if variable name is "Unstack_2", result will be Pair("Unstack_2", 0)
		''' - if variable name is "Unstack_2:12", result will be Pair("Unstack_2", 12)
		''' </summary>
		''' <param name="varName">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<String, Integer> parseVariable(@NonNull String varName)
		public static Pair(Of String, Integer) parseVariable( String varName)
		If True Then
			If Not varName.contains(":") Then
				Return Pair.pairOf(varName, 0)
			Else
				Dim split As val = varName.Split(":")
				Dim index As val = Convert.ToInt32(split(split.length - 1))
				If split.length = 2 Then
					Return Pair.pairOf(split(0), index)
				Else
					Dim builder As val = New StringBuilder()
					For e As Integer = 0 To split.length - 2
						builder.append(split(e))

						If e < split.length - 2 Then
							builder.append(":")
						End If
					Next e

					Return Pair.pairOf(builder.ToString(), index)
				End If
			End If
		End If

		''' <summary>
		''' This method exports the current SameDiff instance into FlatBuffers format, returning the array ops and
		''' all arrays as a ByteBuffer containing the FlatBuffers format data
		''' </summary>
		''' <param name="configuration">       - ExecutorConfiguration to be embedded into serialized graph </param>
		''' <param name="includeUpdaterState"> If true: include the updater state (state for updaters such as Adam, Nesterov, AdaGrad etc) </param>
		''' <returns> a ByteBuffer holding the exported FlatBuffers representation of the graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public java.nio.ByteBuffer asFlatBuffers(@NonNull ExecutorConfiguration configuration, boolean includeUpdaterState)
		public ByteBuffer asFlatBuffers( ExecutorConfiguration configuration, Boolean includeUpdaterState)
		If True Then
			Return asFlatBuffers(0, configuration, includeUpdaterState)
		End If

		''' <summary>
		''' This method exports the current SameDiff instance into FlatBuffers format, returning the array ops and
		''' all arrays as a ByteBuffer containing the FlatBuffers format data
		''' </summary>
		''' <param name="configuration">       - ExecutorConfiguration to be embedded into serialized graph </param>
		''' <param name="includeUpdaterState"> If true: include the updater state (state for updaters such as Adam, Nesterov, AdaGrad etc) </param>
		''' <returns> a ByteBuffer holding the exported FlatBuffers representation of the graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public java.nio.ByteBuffer asFlatBuffers(long graphId, @NonNull ExecutorConfiguration configuration, boolean includeUpdaterState)
		public ByteBuffer asFlatBuffers(Long graphId, ExecutorConfiguration configuration, Boolean includeUpdaterState)
		If True Then
			Nd4j.Executioner.commit()
			Dim bufferBuilder As val = New FlatBufferBuilder(1024)
			Dim idCounter As val = New AtomicInteger(0)

			Dim flatVariables As val = New List(Of Integer)()
			Dim flatOffsets As val = New List(Of Integer)()
			Dim flatNodes As val = New List(Of Integer)()

			' first of all we build VariableSpace dump
			Dim variableList As val = New List(Of )(variables())
			Dim reverseMap As val = New LinkedHashMap(Of String, Integer)()
			Dim forwardMap As val = New LinkedHashMap(Of String, Integer)()
			Dim framesMap As val = New LinkedHashMap(Of String, Integer)()

			Dim idx As Integer = 0
			Dim idxForOps As val = New IdentityHashMap(Of DifferentialFunction, Integer)()
			Dim allVars As IList(Of SDVariable) = variables()
			For Each variable As SDVariable In allVars
				Dim arr As INDArray = If(variable.getVariableType() = VariableType.ARRAY, Nothing, variable.Arr)
				log.trace("Exporting variable: [{}]", variable.name())

				'If variable is the output of some op - let's use the ONE index for exporting, and properly track the output
				' numbers. For example, unstack(x) -> y0, y1, y2 -> the y's should be say (3,0), (3,1), (3,2) NOT (4,0), (5,0), (6,0)
				Dim varName As String = variable.name()
				Dim varIdx As Integer
				Dim outputNum As Integer
				If variables(varName).getOutputOfOp() IsNot Nothing Then
					'This variable is the output of a node
					Dim df As DifferentialFunction = ops(variables(varName).getOutputOfOp()).getOp()
					If Not idxForOps.containsKey(df) Then
						varIdx = idCounter.incrementAndGet()
						idxForOps.put(df, varIdx)
					Else
						varIdx = idxForOps.get(df)
					End If
					Dim outNames() As String = df.outputVariablesNames()
					outputNum = ArrayUtils.IndexOf(outNames, varName)
					Preconditions.checkState(outputNum >= 0, "Variable name ""%s"" not found in list of outputs: %s", varName, outNames)
				Else
					varIdx = idCounter.incrementAndGet()
					outputNum = 0
				End If


				reverseMap.put(variable.name(), varIdx)

				log.trace("Adding [{}] as [{}]", variable.name(), varIdx)
				Dim shape As Integer = 0
				Dim name As Integer = bufferBuilder.createString(variable.name())
				Dim array As Integer = 0
				Dim id As Integer = IntPair.createIntPair(bufferBuilder, varIdx, outputNum)
'JAVA TO VB CONVERTER NOTE: The variable varType was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim varType_Conflict As SByte = CSByte(Math.Truncate(variable.getVariableType().ordinal()))
				If variable.Constant OrElse variable.PlaceHolder OrElse variable.getVariableType() = VariableType.VARIABLE Then
					'Don't export array type (i.e., activations), these are always replaced/re-calculated on each step
					array = If(arr Is Nothing, 0, arr.toFlatArray(bufferBuilder))
				End If

				If variable.getVariableType() = VariableType.PLACEHOLDER Then
					Dim shp As val = variable.Shape
					If shp IsNot Nothing Then
						'Some models may have no shape defined, not ever a placeholder type shape
						shape = FlatVariable.createShapeVector(bufferBuilder, shp)
					End If
				End If

				Dim controlDeps As Integer = 0
				Dim controlDepsForOp As Integer = 0
				Dim controlDepsForVar As Integer = 0
				Dim v As Variable = variables(varName)

				Dim cds() As Integer = FlatBuffersMapper.mapOrNull(v.getControlDeps(), bufferBuilder)
				If cds IsNot Nothing Then
					controlDeps = FlatVariable.createControlDepsVector(bufferBuilder, cds)
				End If

				Dim cdsForOp() As Integer = FlatBuffersMapper.mapOrNull(v.getControlDepsForOp(), bufferBuilder)
				If cdsForOp IsNot Nothing Then
					controlDepsForOp = FlatVariable.createControlDepForOpVector(bufferBuilder, cdsForOp)
				End If

				Dim cdsForVar() As Integer = FlatBuffersMapper.mapOrNull(v.getControlDepsForVar(), bufferBuilder)
				If cdsForVar IsNot Nothing Then
					controlDepsForVar = FlatVariable.createControlDepsForVarVector(bufferBuilder, cdsForVar)
				End If


'JAVA TO VB CONVERTER NOTE: The variable flatVariable was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim flatVariable_Conflict As Integer = FlatVariable.createFlatVariable(bufferBuilder, id, name, FlatBuffersMapper.getDataTypeAsByte(variable.dataType()), shape, array, -1, varType_Conflict, controlDeps, controlDepsForOp, controlDepsForVar)
				flatVariables.add(flatVariable_Conflict)
			Next variable

			'add functions
			For Each op As SameDiffOp In ops_Conflict.Values
				Dim func As DifferentialFunction = op.Op
				Dim fnId As Integer? = idxForOps.get(func)
				flatNodes.add(FlatBuffersMapper.asFlatNode(Me, func, bufferBuilder, variableList, reverseMap, forwardMap, framesMap, idCounter, fnId))
			Next op

			Dim outputsOffset As Integer = FlatGraph.createVariablesVector(bufferBuilder, Ints.toArray(flatOffsets))
			Dim variablesOffset As Integer = FlatGraph.createVariablesVector(bufferBuilder, Ints.toArray(flatVariables))
			Dim nodesOffset As Integer = FlatGraph.createNodesVector(bufferBuilder, Ints.toArray(flatNodes))

			Dim numPlaceholders As Integer = 0
			For Each v As SDVariable In variables()
				If v.PlaceHolder Then
					numPlaceholders += 1
				End If
			Next v

			Dim placeholderOffsets(numPlaceholders - 1) As Integer
			If numPlaceholders > 0 Then
				Dim i As Integer = 0
				For Each v As SDVariable In variables()
					If Not v.PlaceHolder Then
						Continue For
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: placeholderOffsets[i++] = bufferBuilder.createString(v.name());
					placeholderOffsets(i) = bufferBuilder.createString(v.name())
						i += 1
				Next v
			End If
			Dim placeholdersOffset As Integer = FlatGraph.createPlaceholdersVector(bufferBuilder, placeholderOffsets)

			Dim lossVars As IList(Of String) = getLossVariables()
			Dim lossVarOffsets(If(lossVars Is Nothing, 0, lossVars.Count) - 1) As Integer
			For i As Integer = 0 To lossVarOffsets.Length - 1
				lossVarOffsets(i) = bufferBuilder.createString(lossVars(i))
			Next i
			Dim lossVarOffset As Integer = FlatGraph.createLossVariablesVector(bufferBuilder, lossVarOffsets)

			Dim trainingConfigOffset As Integer = 0
			Dim updaterStateOffset As Integer = 0
			If trainingConfig_Conflict IsNot Nothing Then
				Dim json As String = trainingConfig_Conflict.toJson()
				trainingConfigOffset = bufferBuilder.createString(json)
			End If
			If includeUpdaterState AndAlso updaterMap IsNot Nothing AndAlso updaterMap.Count > 0 Then
				Dim updaterOffsets(updaterMap.Count - 1) As Integer
				Dim updaterNum As Integer = 0
				For Each g As KeyValuePair(Of String, GradientUpdater) In updaterMap.SetOfKeyValuePairs()
					Dim paramNameOffset As Integer = bufferBuilder.createString(g.Key)
					Dim stateKeyOffset As Integer = 0
					Dim stateValuesOffset As Integer = 0
					Dim state As IDictionary(Of String, INDArray) = g.Value.getState()
					If state IsNot Nothing AndAlso state.Count > 0 Then
						Dim keysOffsets(state.Count - 1) As Integer
						Dim valuesOffsets(state.Count - 1) As Integer
						Dim i As Integer = 0
						For Each e As KeyValuePair(Of String, INDArray) In state.SetOfKeyValuePairs()
							keysOffsets(i) = bufferBuilder.createString(e.Key)
							valuesOffsets(i) = e.Value.toFlatArray(bufferBuilder)
							i += 1
						Next e

						stateKeyOffset = UpdaterState.createUpdaterStateKeysVector(bufferBuilder, keysOffsets)
						stateValuesOffset = UpdaterState.createUpdaterStateValuesVector(bufferBuilder, valuesOffsets)
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: updaterOffsets[updaterNum++] = UpdaterState.createUpdaterState(bufferBuilder, paramNameOffset, stateKeyOffset, stateValuesOffset);
					updaterOffsets(updaterNum) = UpdaterState.createUpdaterState(bufferBuilder, paramNameOffset, stateKeyOffset, stateValuesOffset)
						updaterNum += 1
				Next g

				updaterStateOffset = FlatGraph.createUpdaterStateVector(bufferBuilder, updaterOffsets)
			End If

			Dim fg As Integer = FlatGraph.createFlatGraph(bufferBuilder, graphId, variablesOffset, nodesOffset, outputsOffset, configuration.getFlatConfiguration(bufferBuilder), placeholdersOffset, lossVarOffset, trainingConfigOffset, updaterStateOffset)
			bufferBuilder.finish(fg)

			SyncLock Me
				For Each e As KeyValuePair(Of String, Integer) In reverseMap.entrySet()
					Me.variables_Conflict(e.Key).setVariableIndex(e.Value)
				Next e
			End SyncLock
			Return bufferBuilder.dataBuffer()
		End If

		''' <summary>
		''' See <seealso cref="asFlatGraph(Long, ExecutorConfiguration, Boolean)"/>.
		''' 
		''' Uses the default <seealso cref="ExecutorConfiguration"/> with output mode as
		''' <seealso cref="OutputMode.VARIABLE_SPACE"/>, execution mode as <seealso cref="ExecutionMode.SEQUENTIAL"/>,
		''' with profiling disabled and gather timings enabled.
		''' </summary>
		public FlatGraph asFlatGraph(Boolean includeUpdaterState)
		If True Then
			Return FlatGraph.getRootAsFlatGraph(Me.asFlatBuffers(includeUpdaterState))
		End If

		''' <summary>
		''' This method returns FlatGraph structure
		''' </summary>
		''' <param name="configuration"> </param>
		''' <param name="includeUpdaterState"> If true: include the updater state (state for updaters such as Adam, Nesterov, AdaGrad etc)
		''' @return </param>
		public FlatGraph asFlatGraph(Long graphId, ExecutorConfiguration configuration, Boolean includeUpdaterState)
		If True Then
			Return FlatGraph.getRootAsFlatGraph(asFlatBuffers(graphId, configuration, includeUpdaterState))
		End If

		''' <summary>
		''' This method exports the current SameDiff instance into FlatBuffers format, returning the array ops and
		''' all arrays as a ByteBuffer containing the FlatBuffers format data
		''' 
		''' Uses the default <seealso cref="ExecutorConfiguration"/> with output mode as
		''' <seealso cref="OutputMode.VARIABLE_SPACE"/>, execution mode as <seealso cref="ExecutionMode.SEQUENTIAL"/>,
		''' with profiling disabled and gather timings enabled.
		''' </summary>
		''' <param name="includeUpdaterState"> If true: include the updater state (state for updaters such as Adam, Nesterov, AdaGrad etc) </param>
		''' <returns> a ByteBuffer holding the exported FlatBuffers representation of the graph </returns>
		public ByteBuffer asFlatBuffers(Boolean includeUpdaterState)
		If True Then
			Dim configuration As val = ExecutorConfiguration.builder().outputMode(OutputMode.VARIABLE_SPACE).executionMode(org.nd4j.autodiff.execution.conf.ExecutionMode.SEQUENTIAL).profilingMode(OpExecutioner.ProfilingMode.DISABLED).gatherTimings(True).build()

			Return asFlatBuffers(configuration, includeUpdaterState)
		End If

		''' <summary>
		''' Save the SameDiff instance to a file. Files can be loaded using <seealso cref="load(File, Boolean)"/>
		''' </summary>
		''' <param name="file">             File to save to </param>
		''' <param name="saveUpdaterState"> If true: save the updater state (arrays etc for Adam, Nesterov, RmsProp etc). If false: don't save
		'''                         the updater state. If you want to continue training after loading your model, this should be true,
		'''                         however may increase the file size significantly.
		'''                         If the network is to be used for inference only, set this to false to save space </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void save(@NonNull File file, boolean saveUpdaterState)
		public void save( File file, Boolean saveUpdaterState)
		If True Then
			Try
				asFlatFile(file, saveUpdaterState)
			Catch e As IOException
				Throw New Exception("Error saving SameDiff instance to file", e)
			End Try
		End If

		''' <summary>
		''' As per <seealso cref="save(File, Boolean)"/> but the serialized SameDiff instance is written to the output stream instead.
		''' Note that this temporarily saves to disk (using <seealso cref="ND4JFileUtils.createTempFile(String, String)"/> then copies all
		''' file bytes to the stream
		''' </summary>
		''' <param name="outputStream"> Stream to write the serialized SameDiff instance to </param>
		''' <param name="saveUpdater">  If true: save the updater state (arrays etc for Adam, Nesterov, RmsProp etc). If false: don't save
		'''                     the updater state. If you want to continue training after loading your model, this should be true,
		'''                     however may increase the file size significantly.
		'''                     If the network is to be used for inference only, set this to false to save space. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void save(@NonNull OutputStream outputStream, boolean saveUpdater)
		public void save( Stream outputStream, Boolean saveUpdater)
		If True Then
			Dim tempFile As File = ND4JFileUtils.createTempFile("SameDiffFile", "temp")
			Try
				save(tempFile, saveUpdater)
				If Not (TypeOf outputStream Is BufferedOutputStream) Then
					outputStream = New BufferedOutputStream(outputStream)
				End If
				Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Using os As System.IO.Stream_Output = outputStream, is As System.IO.Stream_Input = new BufferedInputStream(new System.IO.FileStream(tempFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
						outputStream, [is] As Stream = New BufferedInputStream(New FileStream(tempFile, FileMode.Open, FileAccess.Read))
							Using os As Stream = outputStream, [is] As Stream
						IOUtils.copy([is], os)
						End Using
				Catch e As IOException
					Throw New Exception("Error writing to output stream (or reading from temp file)", e)
				End Try
			Finally
				tempFile.delete()
			End Try
		End If

		''' <summary>
		''' Load the SameDiff instance previously saved with <seealso cref="save(File, Boolean)"/>
		''' </summary>
		''' <param name="file">             The file to load the network from </param>
		''' <param name="loadUpdaterState"> If true - load the updater state (history etc for updaters such as Adam, Nesterov momentum, RMSProp etc).
		'''                         For inference only, this should be false, as the updater state will take more memory, but
		'''                         is not required for training.
		'''                         If the network is to be trained further, this should be true.
		'''                         The updater state can only be loaded if it was saved with the network. </param>
		''' <returns> The loaded SameDiff network </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static SameDiff load(@NonNull File file, boolean loadUpdaterState)
		public static SameDiff load( File file, Boolean loadUpdaterState)
		If True Then
			Try
				Return fromFlatFile(file, loadUpdaterState)
			Catch e As IOException
				Throw New Exception("Error loading SameDiff instance from file", e)
			End Try
		End If

		''' <summary>
		''' As per <seealso cref="load(File, Boolean)"/> but the SameDiff instance
		''' </summary>
		''' <param name="is">               Input stream to load the saved network from </param>
		''' <param name="loadUpdaterState"> If true - load the updater state (history etc for updaters such as Adam, Nesterov momentum, RMSProp etc).
		'''                         For inference only, this should be false, as the updater state will take more memory, but
		'''                         is not required for training.
		'''                         If the network is to be trained further, this should be true.
		'''                         The updater state can only be loaded if it was saved with the network. </param>
		''' <returns> The loaded SameDiff network </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static SameDiff load(@NonNull InputStream is, boolean loadUpdaterState)
		public static SameDiff load( Stream [is], Boolean loadUpdaterState)
		If True Then
			Dim tempFile As File = ND4JFileUtils.createTempFile("SameDiffFile", "temp")
			Try
				Using os As Stream = New BufferedOutputStream(New FileStream(tempFile, FileMode.Create, FileAccess.Write))
					IOUtils.copy([is], os)
				End Using
				Return fromFlatFile(tempFile, loadUpdaterState)
			Catch e As IOException
				Throw New Exception("Error loading SameDiff instance from file", e)
			Finally
				tempFile.delete()
			End Try
		End If

		''' <summary>
		''' This method converts SameDiff instance to FlatBuffers and saves it to file which can be restored later<br>
		''' This includes the updater state, if applicable.
		''' 
		''' Uses the default <seealso cref="ExecutorConfiguration"/> with output mode as
		''' <seealso cref="OutputMode.VARIABLE_SPACE"/>, execution mode as <seealso cref="ExecutionMode.SEQUENTIAL"/>,
		''' with profiling disabled and gather timings enabled.
		''' </summary>
		''' <param name="file"> File to save the FlatBuffers serialized graph (including arrays) to </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void asFlatFile(@NonNull File file) throws IOException
		public void asFlatFile( File file) throws IOException
		If True Then
			asFlatFile(file, True)
		End If

		''' <summary>
		''' See <seealso cref="asFlatFile(File, ExecutorConfiguration, Boolean)"/>.
		''' 
		''' Uses the default <seealso cref="ExecutorConfiguration"/> with output mode as
		''' <seealso cref="OutputMode.VARIABLE_SPACE"/>, execution mode as <seealso cref="ExecutionMode.SEQUENTIAL"/>,
		''' with profiling disabled and gather timings enabled.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void asFlatFile(@NonNull File file, boolean withUpdaterState) throws IOException
		public void asFlatFile( File file, Boolean withUpdaterState) throws IOException
		If True Then
			Dim fb As val = asFlatBuffers(withUpdaterState)
			Dim offset As val = fb.position()

			Dim array As val = fb.array()

			Using fos As val = New FileStream(file, FileMode.Create, FileAccess.Write), bos As val = New BufferedOutputStream(fos), dos As val = New DataOutputStream(bos)
				dos.write(array, offset, array.length - offset)
			End Using
		End If

		''' <summary>
		''' This method converts SameDiff instance to FlatBuffers and saves it to file which can be restored later
		''' </summary>
		''' <param name="file">                File to save the FlatBuffers serialized graph (including arrays) to </param>
		''' <param name="includeUpdaterState"> If true: include the updater state (state for updaters such as Adam, Nesterov, AdaGrad etc) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void asFlatFile(@NonNull File file, @NonNull ExecutorConfiguration configuration, boolean includeUpdaterState) throws IOException
		public void asFlatFile( File file, ExecutorConfiguration configuration, Boolean includeUpdaterState) throws IOException
		If True Then
			Dim fb As val = asFlatBuffers(configuration, includeUpdaterState)
			Dim offset As val = fb.position()

			Dim array As val = fb.array()

			Using fos As val = New FileStream(file, FileMode.Create, FileAccess.Write), bos As val = New BufferedOutputStream(fos), dos As val = New DataOutputStream(bos)
				dos.write(array, offset, array.length - offset)
			End Using
		End If


		''' <summary>
		''' Create a <seealso cref="SameDiff"/> instance from a file, including the updater state
		''' The method to save the file is <seealso cref="save(File, Boolean)"/>
		''' </summary>
		''' <param name="file"> the file to load from </param>
		''' <returns> the loaded same diff instance </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static SameDiff fromFlatFile(@NonNull File file) throws IOException
		public static SameDiff fromFlatFile( File file) throws IOException
		If True Then
			Return fromFlatFile(file, True)
		End If

		''' <summary>
		''' Create a <seealso cref="SameDiff"/> instance from a file, optionally also loading the updater state
		''' The method to save the file is <seealso cref="save(File, Boolean)"/>
		''' </summary>
		''' <param name="file">             the file to load from </param>
		''' <param name="loadUpdaterState"> If true, load the updater state (Adam etc state). For training, use true. For inference, use false </param>
		''' <returns> the loaded same diff instance </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static SameDiff fromFlatFile(@NonNull File file, boolean loadUpdaterState) throws IOException
		public static SameDiff fromFlatFile( File file, Boolean loadUpdaterState) throws IOException
		If True Then
			Dim bytes() As SByte
			Using [is] As Stream = New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
				bytes = IOUtils.toByteArray([is])
			End Using

			Dim bbIn As ByteBuffer = ByteBuffer.wrap(bytes)
			Return fromFlatBuffers(bbIn, loadUpdaterState)
		End If

		''' <summary>
		''' Create a <seealso cref="SameDiff"/>
		''' instance from a byte buffers
		''' instance.
		''' 
		''' See <seealso cref="fromFlatBuffers(ByteBuffer, Boolean)"/>.  Loads updater state (loadUpdaterState is true).
		''' </summary>
		''' <param name="bbIn"> the input byte buffer </param>
		''' <returns> the created samediff instance </returns>
		''' <exception cref="IOException"> </exception>
		public static SameDiff fromFlatBuffers(ByteBuffer bbIn) throws IOException
		If True Then
			Return fromFlatBuffers(bbIn, True)
		End If

		''' <summary>
		''' Create a <seealso cref="SameDiff"/>
		''' instance from a byte buffers
		''' instance.
		''' </summary>
		''' <param name="bbIn"> the input byte buffer </param>
		''' <param name="loadUpdaterState"> If true, load the updater state (Adam etc state). For training, use true. For inference, use false </param>
		''' <returns> the created samediff instance </returns>
		''' <exception cref="IOException"> </exception>
		public static SameDiff fromFlatBuffers(ByteBuffer bbIn, Boolean loadUpdaterState) throws IOException
		If True Then

			Dim fg As FlatGraph = FlatGraph.getRootAsFlatGraph(bbIn)

			Dim numOps As Integer = fg.nodesLength()
			Dim numVars As Integer = fg.variablesLength()
			Dim ops As IList(Of FlatNode) = New List(Of FlatNode)(numOps)
			For i As Integer = 0 To numOps - 1
				ops.Add(fg.nodes(i))
			Next i
			Dim vars As IList(Of FlatVariable) = New List(Of FlatVariable)(numVars)
			For i As Integer = 0 To numVars - 1
				vars.Add(fg.variables(i))
			Next i

	'        FlatConfiguration conf = fg.configuration();

	'         Reconstruct the graph
	'        We'll do the reconstruction manually here, rather than using sd.var(...), so that we have more control
	'        over the final result.
	'         

			Dim sd As SameDiff = SameDiff.create()

			'Reconstruct placeholders
			Dim numPlaceholders As Integer = fg.placeholdersLength()
			Dim ph As ISet(Of String) = New LinkedHashSet(Of String)()
			For i As Integer = 0 To numPlaceholders - 1
				ph.Add(fg.placeholders(i))
			Next i

			'Reconstruct variables:
			Dim varNodeIds As IDictionary(Of Integer, SDVariable) = New Dictionary(Of Integer, SDVariable)()
			Dim variablesByNodeAndOutNum As IDictionary(Of Pair(Of Integer, Integer), SDVariable) = New Dictionary(Of Pair(Of Integer, Integer), SDVariable)()
			Dim variablesByName As IDictionary(Of String, IList(Of SDVariable)) = New Dictionary(Of String, IList(Of SDVariable))()
			For Each v As FlatVariable In vars
				Dim shapeLength As Integer = v.shapeLength()
				Dim shape(shapeLength - 1) As Long
				For i As Integer = 0 To shapeLength - 1
					shape(i) = v.shape(i)
				Next i

				Dim n As String = v.name()

				Dim dtypeByte As SByte = v.dtype()
				Dim dtype As DataType = FlatBuffersMapper.getDataTypeFromByte(dtypeByte)

				'TODO Infer this properly! Could be constant, etc.
				Dim vt As VariableType = System.Enum.GetValues(GetType(VariableType))(v.variabletype())
				Dim var As New SDVariable(n, vt, sd, shape, dtype)
				sd.variables_Conflict(n) = Variable.builder().name(n).variable(var).build()
				Dim v2 As Variable = sd.variables_Conflict(n)

				'Reconstruct control dependencies
				If v.controlDepsLength() > 0 Then
					Dim num As Integer = v.controlDepsLength()
					Dim l As IList(Of String) = New List(Of String)(num)
					For i As Integer = 0 To num - 1
						l.Add(v.controlDeps(i))
					Next i
					v2.setControlDeps(l)
				End If
				If v.controlDepForOpLength() > 0 Then
					Dim num As Integer = v.controlDepForOpLength()
					Dim l As IList(Of String) = New List(Of String)(num)
					For i As Integer = 0 To num - 1
						l.Add(v.controlDepForOp(i))
					Next i
					v2.setControlDepsForOp(l)
				End If

				If v.controlDepsForVarLength() > 0 Then
					Dim num As Integer = v.controlDepsForVarLength()
					Dim l As IList(Of String) = New List(Of String)(num)
					For i As Integer = 0 To num - 1
						l.Add(v.controlDepsForVar(i))
					Next i
					v2.setControlDepsForVar(l)
				End If



				Dim fa As FlatArray = v.ndarray()
				If fa IsNot Nothing AndAlso vt <> VariableType.ARRAY Then
					Dim arr As INDArray
					Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
						arr = Nd4j.createFromFlatArray(fa)
					End Using
					sd.setArrayForVariable(n, arr)
				End If

				Dim id As IntPair = v.id() 'First value: node (op) id. Second: output number
				variablesByNodeAndOutNum(New Pair(Of )(id.first(), id.second())) = var

				If Not variablesByName.ContainsKey(n) Then
					variablesByName(n) = New List(Of SDVariable)()
				End If

				Dim list As IList(Of SDVariable) = variablesByName(n)
				list.Add(var)
			Next v

			'Reconstruct ops:
			For Each fn As FlatNode In ops
				Dim df As DifferentialFunction = FlatBuffersMapper.fromFlatNode(fn)
				Dim name As String = fn.name()
				df.setSameDiff(sd)
				df.setOwnName(name)
				If sd.ops_Conflict.ContainsKey(name) Then
					sd.ops_Conflict(name).setOp(df)
				Else
					sd.ops_Conflict(name) = SameDiffOp.builder().name(name).op(df).build()
				End If

				Dim outLength As Integer = fn.outputLength()
				Dim outs(outLength - 1) As Integer
				For i As Integer = 0 To outLength - 1
					outs(i) = fn.output(i)
				Next i

				Dim opId As Integer = fn.id()

				'Work out inputs and outputs:
				Dim output(fn.outputLength() - 1) As Integer
				For i As Integer = 0 To output.Length - 1
					output(i) = fn.output(i)
				Next i
				Dim input(fn.inputLength() - 1) As Integer
				For i As Integer = 0 To input.Length - 1
					input(i) = fn.input(i)
				Next i
				Dim inputPaired(fn.inputPairedLength() - 1) As IntPair
				Dim intPairList As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer))()
				For i As Integer = 0 To inputPaired.Length - 1
					inputPaired(i) = fn.inputPaired(i)
					intPairList.Add(New Pair(Of Integer, Integer)(inputPaired(i).first(), inputPaired(i).second()))
				Next i

				Dim inputNames(inputPaired.Length - 1) As String
				For i As Integer = 0 To inputPaired.Length - 1
					Dim nodeId As Integer = inputPaired(i).first()
					Dim nodeOutNum As Integer = inputPaired(i).second()
					Dim varIn As SDVariable = variablesByNodeAndOutNum(New Pair(Of )(nodeId, nodeOutNum))
					If varIn Is Nothing Then
						'The variable corresponding to this op was not
					End If
					inputNames(i) = varIn.name()
				Next i
				Dim op As SameDiffOp = sd.ops_Conflict(df.getOwnName())
				op.InputsToOp = New List(Of String) From {inputNames}

				'Reconstruct control dependencies
				If fn.controlDepsLength() > 0 Then
					Dim l As Integer = fn.controlDepsLength()
					Dim list As IList(Of String) = New List(Of String)(l)
					For i As Integer = 0 To l - 1
						list.Add(fn.controlDeps(i))
					Next i
					op.ControlDeps = list
				End If

				If fn.varControlDepsLength() > 0 Then
					Dim l As Integer = fn.varControlDepsLength()
					Dim list As IList(Of String) = New List(Of String)(l)
					For i As Integer = 0 To l - 1
						list.Add(fn.varControlDeps(i))
					Next i
					op.VarControlDeps = list
				End If

				If fn.controlDepForLength() > 0 Then
					Dim l As Integer = fn.controlDepForLength()
					Dim list As IList(Of String) = New List(Of String)(l)
					For i As Integer = 0 To l - 1
						list.Add(fn.controlDepFor(i))
					Next i
					op.ControlDepFor = list
				End If


				'Record that input variables are input to this op
				For Each inName As String In inputNames
					Dim v As Variable = sd.getVariables().get(inName)
					If v.getInputsForOp() Is Nothing Then
						v.setInputsForOp(New List(Of String)())
					End If
					If Not v.getInputsForOp().contains(df.getOwnName()) Then
						v.getInputsForOp().add(df.getOwnName())
					End If
				Next inName

				Dim varsForOp As IList(Of SDVariable) = variablesByName(name)

				'Can't assume that variables for the op have all been defined. For example, if we export before execution in SameDiff
				'In theory, we can reconstruct the output variables (minus names) if we know the number of op outputs
				'And we can calculate the op outputs - in most cases - after the op has been created and parameters set
				Dim numOutputs As Integer = df.NumOutputs
				If numOutputs <= 0 Then
					numOutputs = fn.outputLength()
				End If

				Dim varNames() As String = Nothing
				If varsForOp IsNot Nothing AndAlso varsForOp.Count = numOutputs Then
					varNames = New String(varsForOp.Count - 1){}
					For i As Integer = 0 To varNames.Length - 1
						varNames(i) = varsForOp(i).name()
						sd.getVariables().get(varNames(i)).setOutputOfOp(df.getOwnName())
					Next i
					sd.ops_Conflict(df.getOwnName()).setOutputsOfOp(java.util.Arrays.asList(varNames))
				Else
					'We're missing some variables...
					Dim outputNamesLength As Integer = fn.outputNamesLength()
					varNames = New String(outputNamesLength - 1){}
					For i As Integer = 0 To outputNamesLength - 1
						Dim n As String = fn.outputNames(i)
						varNames(i) = n
						If Not sd.variables_Conflict.ContainsKey(n) Then
							'Need to create the variable - perhaps it wasn't exported. Note output of node -> can only be VARIABLE type
							Dim var As New SDVariable(n, VariableType.VARIABLE, sd, Nothing, Nothing)
							sd.variables_Conflict(n) = Variable.builder().name(n).variable(var).build()
							variablesByNodeAndOutNum(New Pair(Of )(opId, i)) = var
						End If
						sd.getVariables().get(varNames(i)).setOutputOfOp(df.getOwnName())
					Next i
					sd.ops_Conflict(df.getOwnName()).setOutputsOfOp(java.util.Arrays.asList(varNames))
				End If

				'Check the op mapping int he variablesByNodeAndOutputNum
				'For multi-output ops, variables will have their own index, not related to the op index
				For i As Integer = 0 To varNames.Length - 1
					Dim p As New Pair(Of Integer, Integer)(opId, i)
					If Not variablesByNodeAndOutNum.ContainsKey(p) Then
						variablesByNodeAndOutNum(p) = sd.getVariable(varNames(i))
					End If
				Next i
			Next fn

			'Reconstruct loss variables
			If fg.lossVariablesLength() > 0 Then
				Dim i As Integer = 0
				Do While i < fg.lossVariablesLength()
					sd.addLossVariable(fg.lossVariables(i))
					i += 1
				Loop
			End If

			'Reconstruct training config
			Dim tc As String = fg.trainingConfig()
			If tc IsNot Nothing Then
				sd.trainingConfig_Conflict = TrainingConfig.fromJson(tc)
			End If

			If loadUpdaterState Then
				'Reconstruct updater state
				If fg.updaterStateLength() > 0 Then
					sd.updaterMap = New Dictionary(Of String, GradientUpdater)()
					Dim n As Integer = fg.updaterStateLength()
					For i As Integer = 0 To n - 1
						Dim us As UpdaterState = fg.updaterState(i)
						Dim name As String = us.paramName()
						Dim nKeys As Integer = us.updaterStateKeysLength()
						Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
						For j As Integer = 0 To nKeys - 1
							Dim key As String = us.updaterStateKeys(j)
							Dim fa As FlatArray = us.updaterStateValues(j)
							Dim stateArr As INDArray = Nd4j.createFromFlatArray(fa)
							m(key) = stateArr
						Next j

						'Initialize the updater
						Dim gu As GradientUpdater = sd.trainingConfig_Conflict.getUpdater().instantiate(m, False)
						sd.updaterMap(name) = gu
					Next i

					sd.initializedTraining = True
				End If
			End If

			Return sd
		End If

		''' <summary>
		''' This method returns a text representation of the "flattened" graph.
		''' </summary>
		''' <returns> String representation of the graph </returns>
		''' <seealso cref= #summary() </seealso>
		public String asFlatPrint()
		If True Then
			Dim sb As val = New StringBuilder()
			Dim fb As val = asFlatBuffers(False)

			Dim graph As val = FlatGraph.getRootAsFlatGraph(fb)

			sb.append(vbLf & "External variables:" & vbLf & vbLf)
			Dim e As Integer = 0
			Do While e < graph.variablesLength()
				Dim var As FlatVariable = graph.variables(e)
				Dim ndarray As INDArray = Nothing
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
					Dim fa As FlatArray = var.ndarray()
					If fa IsNot Nothing Then
						ndarray = Nd4j.createFromFlatArray(fa)
					End If
				End Using

				sb.append(var.id().first()).append(":<").append(var.name()).append("> ")
				If ndarray Is Nothing Then
					sb.append("<no array>").append("; Values: ").append("<no array>").append(";" & vbLf)
				Else
					sb.append(java.util.Arrays.toString(ndarray.shapeInfoDataBuffer().asInt())).append("; Values: ")
					If ndarray.data() Is Nothing Then
						'Empty array
						sb.append("<empty array>")
					ElseIf ndarray.dataType() = DataType.UTF8 Then
						sb.append("<string array>")
					Else
						If ndarray.length() < 50 Then
							sb.append(java.util.Arrays.toString(ndarray.data().asFloat()).replaceAll(" ", ""))
						Else
							'Array is too long - only tak. last few values...
							sb.append("[")
							For i As Integer = 0 To 49
								If i > 0 Then
									sb.append(",")
								End If
								sb.append(ndarray.data().getFloat(i))
							Next i
							sb.append("]")
						End If
					End If
					sb.append(";" & vbLf)
				End If

				e += 1
			Loop

			Dim map As val = Nd4j.Executioner.getCustomOperations()


			sb.append(vbLf & "Ops sequence:" & vbLf & vbLf)
			e = 0
			Do While e < graph.nodesLength()
				Dim node As FlatNode = graph.nodes(e)

				log.info("{}:<{}>", node.id(), node.name())
				sb.append(node.id()).append(":<").append(node.name()).append("> ").append(FlatBuffersMapper.getTypeFromByte(node.opType()))

				If FlatBuffersMapper.getTypeFromByte(node.opType()) <> Op.Type.CUSTOM Then
					sb.append(": ").append(node.opNum())
				Else
					Dim keys As val = map.keySet()
					Dim opName As String = Nothing
					For Each k As val In keys
						Dim d As val = map.get(k)
						If d.getHash() = node.opNum() Then
							opName = k
						End If
					Next k

					If opName Is Nothing Then
						opName = "unknown"
					End If

					sb.append(": ").append(opName)
				End If

				sb.append("; Inputs: {")

				Dim i As Integer = 0
				Do While i < node.inputPairedLength()
					Dim pair As IntPair = node.inputPaired(i)

					sb.append("[").append(pair.first()).append(":").append(pair.second()).append("]")

					If i < node.inputPairedLength() - 1 Then
						sb.append(", ")
					End If
					i += 1
				Loop

				sb.append("};")
				sb.append(" OpNum: {").append(node.opNum()).append("};")

				sb.append(vbLf)
				e += 1
			Loop


			Return sb.ToString()
		End If

		''' <summary>
		''' Generate and return a String representation of the current SameDiff instance<br>
		''' Reports variables, ops, SameDiff function instances, and (where possible) array shapes.<br>
		''' For ops, the input and output variables are reported.<br>
		''' For variables, the ops that they are inputs to - or outputs of - are also reported
		''' </summary>
		''' <returns> A String representation of the SameDiff instance </returns>
		public String summary()
		If True Then

			Dim varMap As IDictionary(Of String, SDVariable) = variableMap()
			Dim functions() As DifferentialFunction = ops()


			Dim countVarsWithArrays As Integer = 0
			For Each s As String In varMap.Keys
				If getArrForVarName(s) IsNot Nothing Then
					countVarsWithArrays += 1
				End If
			Next s

			Dim sb As New StringBuilder()
			Dim format As String = "%-25s%-20s"
			sb.Append("--- Summary ---" & vbLf)
			sb.Append(String.format(format, "Variables:", varMap.Count)).Append(" (").Append(countVarsWithArrays).Append(" with arrays)").Append(vbLf).Append(String.format(format, "Functions:", functions.Length)).Append(vbLf).Append(String.format(format, "SameDiff Function Defs:", sameDiffFunctionInstances.Count)).Append(vbLf).Append("Loss function variables: ").Append(getLossVariables()).Append(vbLf & vbLf)

			sb.Append("--- Variables ---" & vbLf)
			'Work out which function - if any - this arg is an output of...
			Dim outputOfFn As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			Dim maxLengthOutputOf As Integer = 22 'Length of "- Output Of Function -"
			Dim maxLengthOfName As Integer = 8 'Length of "- Name -"
			For Each s As String In varMap.Keys
				Dim outputOf As String = Nothing
				For Each op As SameDiffOp In ops_Conflict.Values
					Dim outputsOfOp As IList(Of String) = op.getOutputsOfOp()
					If outputsOfOp IsNot Nothing AndAlso outputsOfOp.Contains(s) Then
						outputOf = op.Name
						Exit For
					End If
				Next op

				If outputOf Is Nothing Then
					outputOf = "<none>"
				Else
					Dim d As DifferentialFunction = getOpById(outputOf)
					outputOf = d.getOwnName() & "(" & d.opName() & ")"
				End If
				outputOfFn(s) = outputOf
				maxLengthOutputOf = Math.Max(maxLengthOutputOf, outputOf.Length)
				maxLengthOfName = Math.Max(maxLengthOfName, s.Length)
			Next s
			maxLengthOutputOf += 2
			maxLengthOfName += 2

			'Create the output for values:
			format = "%-" & maxLengthOfName & "s%-20s%-20s%-20s%-" & maxLengthOutputOf & "s%-20s"
			sb.Append(String.format(format, "- Name -", "- Array Shape -", "- Variable Type -", "- Data Type-", "- Output Of Function -", "- Inputs To Functions -")).Append(vbLf)
			For Each s As String In varMap.Keys
				Dim arr As INDArray = getArrForVarName(s)
				Dim arrayShape As String = "-"
				If arr IsNot Nothing Then
					arrayShape = java.util.Arrays.toString(arr.shape())
				ElseIf varMap(s).isPlaceHolder() Then
					Dim v As SDVariable = varMap(s)
					Dim phShape() As Long = v.placeholderShape()
					If phShape IsNot Nothing Then
						arrayShape = java.util.Arrays.toString(phShape)
					End If
				End If
'JAVA TO VB CONVERTER NOTE: The variable varType was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim varType_Conflict As String = getVariable(s).getVariableType().ToString()
				Dim dtype As String = getVariable(s).dataType().ToString()

				Dim argNames As IList(Of String) = variables(s).getInputsForOp()
				Dim dfArrStr As String = ""
				If argNames IsNot Nothing Then
					dfArrStr = argNames.ToString()
				End If

				Dim outputOfStr As String = outputOfFn(s)

				sb.Append(String.format(format, s, arrayShape, varType_Conflict, dtype, outputOfStr, dfArrStr)).Append(vbLf)
			Next s

			sb.Append(vbLf & vbLf & "--- Functions ---" & vbLf)

			'First: work out the amount of space we need for inputs and outputs...
			Dim dfInputStr As IList(Of String) = New List(Of String)()
			Dim dfOutputStr As IList(Of String) = New List(Of String)()
			Dim maxInLength As Integer = 10 'Length of "- Inputs -"
			Dim maxOutLength As Integer = 11 'Length of "- Outputs -"
			Dim maxOpNameLength As Integer = 17 'Default to min of 17 - length of "- Function Name -"
			Dim maxDfClassNameLength As Integer = 10 'Default to min of 10
			For Each df As DifferentialFunction In functions
				Dim argNames() As String = df.argNames()
				Dim outNames() As String = df.outputVariablesNames()

				Dim argStr As String = java.util.Arrays.toString(argNames)
				Dim outStr As String = java.util.Arrays.toString(outNames)

				maxInLength = Math.Max(maxInLength, argStr.Length)
				maxOutLength = Math.Max(maxOutLength, outStr.Length)

				dfInputStr.Add(argStr)
				dfOutputStr.Add(outStr)

				Dim name As String = If(df.getOwnName() Is Nothing, df.opName(), df.getOwnName())
				maxOpNameLength = Math.Max(maxOpNameLength, name.Length)
				maxDfClassNameLength = Math.Max(maxDfClassNameLength, df.GetType().Name.Length)
			Next df
			'Extra padding space
			maxInLength += 2
			maxOutLength += 2
			maxOpNameLength += 2
			maxDfClassNameLength += 2


			format = "%-5s%-" & maxOpNameLength & "s%-" & maxDfClassNameLength & "s%-" & maxInLength & "s%-" & maxOutLength & "s"
			sb.Append(String.format(format, "", "- Function Name -", "- Op -", "- Inputs -", "- Outputs -")).Append(vbLf)
			For i As Integer = 0 To functions.Length - 1
				Dim df As DifferentialFunction = functions(i)
				Dim fnName As String = If(df.getOwnName() Is Nothing, df.opName(), df.getOwnName())

				sb.Append(String.format(format, i, fnName, df.GetType().Name, dfInputStr(i), dfOutputStr(i))).Append(vbLf)
			Next i

			If sameDiffFunctionInstances.Count > 0 Then
				sb.Append(vbLf & vbLf & "--- SameDiff Defined Functions ---" & vbLf)
				format = "%-20s%-15s%-15s%-15s"
				sb.Append(String.format(format, "- Name -", "- Variables -", "- Functions -", "- Fn Defs -")).Append(vbLf)
				For Each e As KeyValuePair(Of String, SameDiff) In sameDiffFunctionInstances.SetOfKeyValuePairs()
					Dim sd As SameDiff = e.Value
					Dim vars As Integer = sd.variableMap().Count
					Dim fns As Integer = (If(sd.ops() Is Nothing, 0, sd.ops().Length))
					Dim defFns As Integer = sd.definedFunctionNames().Count

					sb.Append(String.format(format, e.Key, vars, fns, defFns)).Append(vbLf)
				Next e
			End If

			Return sb.ToString()
		End If

		''' <summary>
		''' For internal use only.
		''' Creates a new distinct block name from baseName.
		''' Block names are used by If and While
		''' </summary>
		public String newBlockName(String baseName)
		If True Then

			If baseName Is Nothing Then
				Return Nothing
			End If

			If Not blockNames.Contains(baseName) Then
				blockNames.Add(baseName)
				Return baseName
			Else
				Dim i As Integer = 1
				Do While blockNames.Contains(baseName & "_" & i)
					i += 1
				Loop
				blockNames.Add(baseName & "_" & i)
				Return baseName & "_" & i
			End If
		End If

		''' <summary>
		''' Import a frozen Tensorflow graph to a new SameDiff graph.
		''' </summary>
		''' <param name="graphFile"> The text or binary file containing the graph </param>
		''' <returns> The imported graph </returns>
		public static SameDiff importFrozenTF(File graphFile)
		If True Then
			Return TFGraphMapper.importGraph(graphFile)
		End If

		''' <summary>
		''' See <seealso cref="importFrozenTF(File)"/>
		''' </summary>
		public static SameDiff importFrozenTF(GraphDef graphDef)
		If True Then
			Return TFGraphMapper.importGraph(graphDef)
		End If


		''' <summary>
		''' See <seealso cref="importFrozenTF(File)"/>
		''' <para>
		''' Again, the input can be text or binary.
		''' </para>
		''' </summary>
		public static SameDiff importFrozenTF(Stream graph)
		If True Then
			Return TFGraphMapper.importGraph(graph)
		End If


		''' <summary>
		''' Generate a new, distinct op name of the form &lt;base&gt;_#.
		''' <para>
		''' Applies name scope if active.
		''' 
		''' </para>
		''' </summary>
		''' <param name="base">  The base name to use </param>
		''' <param name="force"> Whether to force the result name to be the same as base. </param>
		public String getOpName(String base, Boolean force)
		If True Then

			base = nameWithScope(base)

			If force AndAlso ops_Conflict.ContainsKey(base) Then
				Throw New System.ArgumentException("Op with name """ & base & """ already exists")
			ElseIf force Then
				Return base
			End If

			Dim start As Integer = 1

			' if we already have a name like "op_2", start from trying "op_3"
			If base.contains("_") AndAlso base.matches(".*_\d+") Then
				' extract number used to generate base
				Dim num As Matcher = Pattern.compile("(.*)_(\d+)").matcher(base)
				' extract argIndex used to generate base
				If num.find() Then
					start = Integer.Parse(num.group(2))
					base = num.group(1)
				End If
			End If

			Dim name As String = base
			Dim i As Integer = start
			Do

				' ensure that there are no variables that look like they are outputs of this op
				Dim varWithName As Boolean = False
				For Each varName As String In variables_Conflict.Keys
					If varName.StartsWith(name & ":", StringComparison.Ordinal) OrElse varName.Equals(name) Then
						varWithName = True
					End If
				Next varName

				If Not ops_Conflict.ContainsKey(name) AndAlso Not varWithName Then
					Exit Do
				End If

				name = base & "_" & i
				i += 1
			Loop
			Return name
		End If

		''' <summary>
		''' See <seealso cref="getOpName(String, Boolean)"/>
		''' force is false
		''' </summary>
		public String getOpName(String base)
		If True Then
			Return getOpName(base, False)
		End If

		''' <summary>
		''' Generate a new, distinct variable name of the form &lt;base&gt;_#[:#].
		''' <para>
		''' Applies name scopes if active.
		''' 
		''' </para>
		''' </summary>
		''' <param name="base">       The base of the name. </param>
		''' <param name="argIndex">   The argument index, used in the ":#".  A value of 0 (or negative) does not include the ":#" part. </param>
		''' <param name="existingOp"> Whether to generate an distinct operation name from base (if false), or just use base (if true). </param>
		public String generateNewVarName(String base, Integer argIndex, Boolean existingOp)
		If True Then

			base = nameWithScope(base)

			If argIndex > 0 AndAlso base.contains(":") Then
				Dim num As Matcher = Pattern.compile("(.*):(\d+)").matcher(base)
				' extract argIndex used to generate base
				If num.find() Then
					argIndex = Integer.Parse(num.group(2)) + 1
					base = num.group(1)
				End If
			End If

			If Not existingOp Then
				base = getOpName(base)
			End If

			If argIndex > 0 Then
				base &= ":" & argIndex
			End If

			If variables_Conflict.ContainsKey(base) Then
				Throw New System.ArgumentException("Variable with name """ & base & """ already exists")
			End If

			Return base
		End If

		''' <summary>
		''' See <seealso cref="generateNewVarName(String, Integer, Boolean)"/>
		''' existingOp is true.
		''' </summary>
		public String generateNewVarName(String base, Integer argIndex)
		If True Then
			Return generateNewVarName(base, argIndex, True)
		End If

		''' <summary>
		''' Returns an unused variable name of the format &lt;base&gt;_#.
		''' 
		''' Intended to be used for custom variables (like weights), arguments and op outputs should use <seealso cref="generateNewVarName(String, Integer)"/>.
		''' </summary>
		public String generateDistinctCustomVariableName(String base)
		If True Then
			If Not variables_Conflict.ContainsKey(base) Then
				Return base
			End If

			Dim inc As Integer = 1

			Do While variables_Conflict.ContainsKey(base & "_" & inc)
				inc += 1
			Loop

			Return base & "_" & inc
		End If


		public String ToString()
		If True Then
			Return "SameDiff(nVars=" & variables_Conflict.Count & ",nOps=" & ops_Conflict.Count & ")"
		End If



		''' <summary>
		''' See <seealso cref="ifCond(String, String, SameDiffNoArgSingleLambda, SameDiffNoArgSingleLambda, SameDiffNoArgSingleLambda)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable ifCond(@NonNull SameDiffNoArgSingleLambda cond, @NonNull SameDiffNoArgSingleLambda trueBody, @NonNull SameDiffNoArgSingleLambda falseBody)
		public SDVariable ifCond( SameDiffNoArgSingleLambda cond, SameDiffNoArgSingleLambda trueBody, SameDiffNoArgSingleLambda falseBody)
		If True Then
			Return ifCond(Nothing, Nothing, cond, trueBody, falseBody)
		End If


		''' <summary>
		''' See <seealso cref="ifCond(String, String, SameDiffNoArgSingleLambda, SameDiffNoArgSingleLambda, SameDiffNoArgSingleLambda)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable ifCond(String ifName, @NonNull SameDiffNoArgSingleLambda cond, @NonNull SameDiffNoArgSingleLambda trueBody, @NonNull SameDiffNoArgSingleLambda falseBody)
		public SDVariable ifCond(String ifName, SameDiffNoArgSingleLambda cond, SameDiffNoArgSingleLambda trueBody, SameDiffNoArgSingleLambda falseBody)
		If True Then
			Return ifCond(Nothing, ifName, cond, trueBody, falseBody)
		End If

		''' <summary>
		''' Constructs a If statement using the tensorflow style control flow operations (Switch and Merge)
		''' 
		''' If the result of cond is true, returns the result of trueBody, otherwise returns the result of falseBody
		''' 
		''' Note that cond and body lambdas are only called once to construct the graph.  The constructed graph is used to evaluate.
		''' 
		''' See <a href="http://download.tensorflow.org/paper/white_paper_tf_control_flow_implementation_2017_11_1.pdf">Tensorflow Control Flow Implementation</a>
		''' </summary>
		''' <param name="outputName"> Name to give the output variable.  If null, doesn't rename </param>
		''' <param name="ifName">  The name of the if block.  If null, uses "if" </param>
		''' <param name="cond">  A lambda evaluating to the if condition </param>
		''' <param name="trueBody">  A lambda to be executed if cond is true (the if block) </param>
		''' <param name="falseBody">  A lambda to be executed if cond is false (the else block) </param>
		''' <returns> The value of trueBody if cond is true, or falseBody if it isn't </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable ifCond(String outputName, String ifName, @NonNull SameDiffNoArgSingleLambda cond, @NonNull SameDiffNoArgSingleLambda trueBody, @NonNull SameDiffNoArgSingleLambda falseBody)
		public SDVariable ifCond(String outputName, String ifName, SameDiffNoArgSingleLambda cond, SameDiffNoArgSingleLambda trueBody, SameDiffNoArgSingleLambda falseBody)
		If True Then

			ifName = newBlockName(If(ifName Is Nothing, "if", ifName))

			Dim ifScope As NameScope = sd.withNameScope(ifName)

			Dim condScope As NameScope = withNameScope("cond")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SDVariable pred = cond.define(this);
			Dim pred As SDVariable = cond.define(Me)
			condScope.Dispose()

			If pred.dataType() <> DataType.BOOL Then
				'cleanup partially added block

				For Each v As SDVariable In getVariablesInScope(ifScope)
					Me.getVariables().remove(v.name())
				Next v

				For Each op As SameDiffOp In Me.getOpsInScope(ifScope)
					For Each [in] As String In op.getInputsToOp()
						Me.removeArgFromOp([in], op.Op)
					Next [in]
					Me.getOps().remove(op.Name)
				Next op


				Throw New System.InvalidOperationException("Can not use " & pred.name() & " as the condition of an If statement, the condition must be a boolean.")
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Map<String, SDVariable[]> switches = new HashMap<>();
			Dim switches As IDictionary(Of String, SDVariable()) = New Dictionary(Of String, SDVariable())()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final @Set<String> declared = org.nd4j.shade.guava.collect.Sets.newHashSet(this.variableMap().keySet());
			Dim declared As ISet(Of String) = Sets.newHashSet(Me.variableMap().Keys)

			Me.addArgumentInterceptor(New ArgumentInterceptorAnonymousInnerClass(Me, pred, switches, declared))
			Dim trueScope As NameScope = Me.withNameScope("trueBody")
			Dim trueOut As SDVariable = trueBody.define(Me)
			Me.removeArgumentInterceptor()

			If declared.Contains(trueOut.name()) Then
				Dim s() As SDVariable = switchOp(trueOut, pred)
				switches(trueOut.name()) = s
				trueOut = s(1)
			End If

			trueScope.Dispose()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final @Set<String> declared2 = org.nd4j.shade.guava.collect.Sets.newHashSet(variableMap().keySet());
			Dim declared2 As ISet(Of String) = Sets.newHashSet(variableMap().Keys)
			sd.addArgumentInterceptor(New ArgumentInterceptorAnonymousInnerClass2(Me, pred, switches, declared2))
			Dim falseScope As NameScope = Me.withNameScope("falseBody")
			Dim falseOut As SDVariable = falseBody.define(Me)
			Me.removeArgumentInterceptor()

			If declared2.Contains(falseOut.name()) Then
				Dim s() As SDVariable = switchOp(falseOut, pred)
				switches(falseOut.name()) = s
				falseOut = s(0)
			End If
			falseScope.Dispose()

			Dim output As SDVariable = merge(trueOut, falseOut)

			ifScope.Dispose()

			Return updateVariableNameAndReference(output, outputName)
		End If

		''' <summary>
		''' See <seealso cref="whileLoop(String[], String, SDVariable[], SameDiffSingleLambda, SameDiffLambda)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable[] whileLoop(@NonNull SDVariable[] loopVars, @NonNull SameDiffSingleLambda cond, @NonNull SameDiffLambda body)
		public SDVariable() whileLoop( SDVariable() loopVars, SameDiffSingleLambda cond, SameDiffLambda body)
		If True Then
			Return whileLoop(Nothing, Nothing, loopVars, cond, body)
		End If

		''' <summary>
		''' See <seealso cref="whileLoop(String[], String, SDVariable[], SameDiffSingleLambda, SameDiffLambda)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable[] whileLoop(String loopName, @NonNull SDVariable[] loopVars, @NonNull SameDiffSingleLambda cond, @NonNull SameDiffLambda body)
		public SDVariable() whileLoop(String loopName, SDVariable() loopVars, SameDiffSingleLambda cond, SameDiffLambda body)
		If True Then
			Return whileLoop(Nothing, loopName, loopVars, cond, body)
		End If


		''' <summary>
		''' Constructs a While loop using the tensorflow style control flow operations (Switch, Merge, Enter, Exit, and NextIteration)
		''' 
		''' Repeatedly executes body on the loop variables and updates them with the results, until cond evaluates to false
		''' 
		''' Note that cond and body lambdas are only called once to construct the graph.  The constructed graph is used for further iterations.
		''' 
		''' See <a href="http://download.tensorflow.org/paper/white_paper_tf_control_flow_implementation_2017_11_1.pdf">Tensorflow Control Flow Implementation</a>
		''' </summary>
		''' <param name="outputNames">  Names to give the output variables.  If null, doesn't rename </param>
		''' <param name="loopName">  The name of the loop block and frame (must be unique).  If null, uses "if" </param>
		''' <param name="loopVars">  Loop variables' inputs </param>
		''' <param name="cond">  A lambda evaluating to the loop condition </param>
		''' <param name="body">  A lambda doing the loop operation and returning the new loop variable values </param>
		''' <returns>  The values of the loop variables once condition is false </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SDVariable[] whileLoop(String[] outputNames, final String loopName, @NonNull SDVariable[] loopVars, @NonNull SameDiffSingleLambda cond, @NonNull SameDiffLambda body)
		public SDVariable() whileLoop(String() outputNames, final String loopName, SDVariable() loopVars, SameDiffSingleLambda cond, SameDiffLambda body)
		If True Then

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String frameName = this.newBlockName(loopName == null ? "while" : loopName);
			Dim frameName As String = Me.newBlockName(If(loopName Is Nothing, "while", loopName))

			Dim loopScope As NameScope = Me.withNameScope(frameName)

			'SDVariable counter = SD.scalar(SD.generateNewVarName("counter", 0), 0);

			Dim entered(loopVars.length - 1) As SDVariable
			For i As Integer = 0 To loopVars.length - 1
				entered(i) = (New Enter(Me, frameName, loopVars(i))).outputVariable()
			Next i

			Dim merged(loopVars.length - 1) As SDVariable
			Dim mergeOps(loopVars.length - 1) As Merge
			For i As Integer = 0 To loopVars.length - 1
				' the second arg will later be replaced with the output of NextIteration
				' but that isn't available yet (and can't be, as it depends on this)
				mergeOps(i) = New Merge(Me, entered(i), entered(i))
				merged(i) = mergeOps(i).outputVariable()
			Next i

			'Merge counterMerge = new Merge(SD, counter, counter);
			'counter = counterMerge.outputVariable();

			Dim condScope As NameScope = Me.withNameScope("cond")
			Dim cond_result As SDVariable = cond.define(Me, merged)
			condScope.Dispose()


			If cond_result.dataType() <> DataType.BOOL Then
				Throw New System.InvalidOperationException("Can not use " & cond_result.name() & " as the condition of an While loop, the condition must be a boolean.")
			End If


'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final @Set<String> alreadyEntered = org.nd4j.shade.guava.collect.Sets.newHashSet();
			Dim alreadyEntered As ISet(Of String) = Sets.newHashSet()
			Dim trueSwitches(loopVars.length - 1) As SDVariable
			Dim exits(loopVars.length - 1) As SDVariable
			For i As Integer = 0 To loopVars.length - 1
				Dim s() As SDVariable = switchOp(merged(i), cond_result)
				trueSwitches(i) = s(1)
				alreadyEntered.Add(s(1).name())
				exits(i) = (New [Exit](Me, s(0))).outputVariable()
			Next i

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final @Set<String> declared = org.nd4j.shade.guava.collect.Sets.newHashSet(this.variableMap().keySet());
			Dim declared As ISet(Of String) = Sets.newHashSet(Me.variableMap().Keys)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Map<String, SDVariable> done = new HashMap<>();
			Dim done As IDictionary(Of String, SDVariable) = New Dictionary(Of String, SDVariable)()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final SameDiff sd = this;
			Dim sd As SameDiff = Me
			Me.addArgumentInterceptor(New ArgumentInterceptorAnonymousInnerClass3(Me, frameName, alreadyEntered, declared, done, sd))

			Dim bodyScope As NameScope = Me.withNameScope("body")
			Dim outs() As SDVariable = body.define(Me, trueSwitches)
			bodyScope.Dispose()
			Me.removeArgumentInterceptor()

			'counter.add(1);

			For i As Integer = 0 To loopVars.length - 1
				Dim n As SDVariable = (New NextIteration(Me, outs(i))).outputVariable()
				mergeOps(i).replaceArg(1,n)
			Next i

			'counterMerge.replaceArg(1, counter);

			loopScope.Dispose()
			Return updateVariableNamesAndReferences(exits, outputNames)
		End If
	End Class

End Namespace