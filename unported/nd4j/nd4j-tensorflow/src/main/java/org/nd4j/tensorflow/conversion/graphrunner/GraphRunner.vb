Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports lombok
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.nd4j.common.primitives
Imports ByteString = org.nd4j.shade.protobuf.ByteString
Imports InvalidProtocolBufferException = org.nd4j.shade.protobuf.InvalidProtocolBufferException
Imports JsonFormat = org.nd4j.shade.protobuf.util.JsonFormat
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports TensorDataType = org.nd4j.tensorflow.conversion.TensorDataType
Imports IOUtils = org.apache.commons.io.IOUtils
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports PointerPointer = org.bytedeco.javacpp.PointerPointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports TensorflowConversion = org.nd4j.tensorflow.conversion.TensorflowConversion
Imports ConfigProto = org.tensorflow.framework.ConfigProto
Imports NodeDef = org.tensorflow.framework.NodeDef
Imports org.bytedeco.tensorflow
Imports org.bytedeco.tensorflow.global.tensorflow

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

Namespace org.nd4j.tensorflow.conversion.graphrunner


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NoArgsConstructor public class GraphRunner implements Closeable
	Public Class GraphRunner
		Implements System.IDisposable

		Private Shared isTfWarmedUp As Boolean = False
		Private Shared isTfWarmingUp As Boolean = False
		Private savedModelConfig As SavedModelConfig
		'the in memory representation parsed from protobuf
		Private graph As TF_Graph
		'the conversion between nd4j and TensorFlow
		Private conversion As TensorflowConversion = TensorflowConversion.Instance
		'a persistent session to be used when running the graph
		Private session As TF_Session
		'the options for the model
		Private options As TF_SessionOptions
		'a status object used
		Private status As TF_Status
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @Singular private List<String> inputOrder,outputOrder;
		Private inputOrder As IList(Of String), outputOrder As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.tensorflow.framework.ConfigProto sessionOptionsConfigProto;
		Private sessionOptionsConfigProto As ConfigProto
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @Singular private Map<String,org.nd4j.tensorflow.conversion.TensorDataType> inputDataTypes,outputDataTypes;
		Private inputDataTypes As IDictionary(Of String, TensorDataType), outputDataTypes As IDictionary(Of String, TensorDataType)
		Private Shared recastGraphDefs As IDictionary(Of Pair(Of TensorDataType, TensorDataType), GraphRunner)

		Shared Sub New()
			recastGraphDefs = New ConcurrentDictionary(Of Pair(Of TensorDataType, TensorDataType), GraphRunner)()
		End Sub


		''' <summary>
		''' The constructor for creating a graph runner via builder </summary>
		''' <param name="inputNames"> the input names to use </param>
		''' <param name="outputNames"> the output names to use </param>
		''' <param name="savedModelConfig"> the saved model configuration to load from (note this can not be used in conjunction
		'''                         with graph path) </param>
		''' <param name="sessionOptionsConfigProto"> the session options for running the model (this maybe null) </param>
		''' <param name="sessionOptionsProtoBytes"> the proto bytes equivalent of the session configuration </param>
		''' <param name="sessionOptionsProtoPath"> the file path to a session configuration proto file </param>
		''' <param name="graph"> the tensorflow graph to use </param>
		''' <param name="graphPath"> the path to the graph </param>
		''' <param name="graphBytes"> the in memory bytes of the graph </param>
		''' <param name="inputDataTypes"> the expected input data types </param>
		''' <param name="outputDataTypes"> the expected output data types </param>



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder public GraphRunner(List<String> inputNames, List<String> outputNames, SavedModelConfig savedModelConfig, org.tensorflow.framework.ConfigProto sessionOptionsConfigProto, byte[] sessionOptionsProtoBytes, File sessionOptionsProtoPath, TF_Graph graph, File graphPath, byte[] graphBytes, Map<String, org.nd4j.tensorflow.conversion.TensorDataType> inputDataTypes, Map<String, org.nd4j.tensorflow.conversion.TensorDataType> outputDataTypes)
		Public Sub New(ByVal inputNames As IList(Of String), ByVal outputNames As IList(Of String), ByVal savedModelConfig As SavedModelConfig, ByVal sessionOptionsConfigProto As ConfigProto, ByVal sessionOptionsProtoBytes() As SByte, ByVal sessionOptionsProtoPath As File, ByVal graph As TF_Graph, ByVal graphPath As File, ByVal graphBytes() As SByte, ByVal inputDataTypes As IDictionary(Of String, TensorDataType), ByVal outputDataTypes As IDictionary(Of String, TensorDataType))
			Try
				If sessionOptionsConfigProto Is Nothing Then
					If sessionOptionsConfigProto IsNot Nothing Then
						Me.sessionOptionsConfigProto = ConfigProto.parseFrom(sessionOptionsProtoBytes)
					ElseIf sessionOptionsProtoPath IsNot Nothing Then
						Dim load() As SByte = FileUtils.readFileToByteArray(sessionOptionsProtoPath)
						Me.sessionOptionsConfigProto = ConfigProto.parseFrom(load)
					End If
				Else
					Me.sessionOptionsConfigProto = sessionOptionsConfigProto
				End If


				Me.inputDataTypes = inputDataTypes
				Me.outputDataTypes = outputDataTypes
				'note that the input and output order, maybe null here
				'if the names are specified, we should defer to those instead
				Me.inputOrder = inputNames
				Me.outputOrder = outputNames
				initOptionsIfNeeded()

				If graph IsNot Nothing Then
					Me.graph = graph
				ElseIf graphBytes IsNot Nothing Then
					Me.graph = conversion.loadGraph(graphBytes, status)
				ElseIf graphPath IsNot Nothing Then
					graphBytes = IOUtils.toByteArray(graphPath.toURI())
					Me.graph = conversion.loadGraph(graphBytes, status)
				Else
					Me.graph = TF_NewGraph()
				End If

				If savedModelConfig IsNot Nothing Then
					Me.savedModelConfig = savedModelConfig
					Dim inputsMap As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
					Dim outputsMap As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()

					Me.session = conversion.loadSavedModel(savedModelConfig, options, Nothing, Me.graph, inputsMap, outputsMap, status)

					If inputOrder Is Nothing OrElse inputOrder.Count = 0 Then
						inputOrder = New List(Of String)(inputsMap.Values)
					End If
					If outputOrder Is Nothing OrElse outputOrder.Count = 0 Then
						outputOrder = New List(Of String)(outputsMap.Values)
					End If

					savedModelConfig.setSavedModelInputOrder(New List(Of )(inputsMap.Values))
					savedModelConfig.setSaveModelOutputOrder(New List(Of )(outputsMap.Values))
					log.info("Loaded input names from saved model configuration " & inputOrder)
					log.info("Loaded output names from saved model configuration " & outputOrder)

				End If


				initSessionAndStatusIfNeeded(graphBytes)
			Catch e As Exception
				Throw New System.ArgumentException("Unable to parse protobuf",e)
			End Try
		End Sub



		''' <summary>
		''' Cast inputs from the original data type
		''' to the target resulting input data type.
		''' This is for when there's a disconnect from the inputs
		''' to the target input data type. This runs a pre cast automatically. </summary>
		''' <param name="inputs"> the inputs to cast </param>
		''' <returns> the re casted input </returns>
		Public Overridable Function recastInputs(ByVal inputs As IDictionary(Of String, TF_Tensor)) As IDictionary(Of String, TF_Tensor)
			Return recastInputs(inputs,inputOrder,inputDataTypes)
		End Function


		''' <summary>
		''' Cast inputs from the original data type
		''' to the target resulting input data type.
		''' This is for when there's a disconnect from the inputs
		''' to the target input data type. This runs a pre cast automatically. </summary>
		''' <param name="inputs"> the inputs to cast </param>
		''' <returns> the re casted input </returns>
		Public Overridable Function recastOutputs(ByVal inputs As IDictionary(Of String, TF_Tensor)) As IDictionary(Of String, TF_Tensor)
			Return recastInputs(inputs,outputOrder,outputDataTypes)
		End Function


		''' <summary>
		''' Automatically recast the input arrays
		''' as the specified types </summary>
		''' <param name="inputs"> the input tensors to recast </param>
		''' <param name="inputOrder"> the order of the input tensors </param>
		''' <param name="inputDataTypes"> the data types to cast to (null means stay the same) </param>
		''' <returns> the new values </returns>
		Public Overridable Function recastInputs(ByVal inputs As IDictionary(Of String, TF_Tensor), ByVal inputOrder As IList(Of String), ByVal inputDataTypes As IDictionary(Of String, TensorDataType)) As IDictionary(Of String, TF_Tensor)
			If inputDataTypes Is Nothing OrElse inputDataTypes.Count = 0 Then

				inputDataTypes = New LinkedHashMap(Of String, TensorDataType)()
				For i As Integer = 0 To inputOrder.Count - 1
					Dim tensorDataType As TensorDataType = TensorDataType.values()(TF_TensorType(inputs(inputOrder(i))))
					Preconditions.checkNotNull(tensorDataType,"Data type of " & TF_TensorType(inputs(inputOrder(i))) & " was null!")
					inputDataTypes(inputOrder(i)) = tensorDataType
				Next i
			End If

			Dim ret As IDictionary(Of String, TF_Tensor) = New Dictionary(Of String, TF_Tensor)()
			For i As Integer = 0 To inputOrder.Count - 1
				Dim currInput As TF_Tensor = inputs(inputOrder(i))
				Dim fromDType As TensorDataType = TensorDataType.values()(TF_TensorType(currInput))
				If fromDType <> inputDataTypes(inputOrder(i)) Then
					Dim oldTensor As TF_Tensor = currInput
					currInput = castTensor(currInput, fromDType, inputDataTypes(inputOrder(i)))
					TF_DeleteTensor(oldTensor)
				End If

				ret(inputOrder(i)) = currInput
			Next i

			Return ret
		End Function

		''' <summary>
		''' Run the graph definition with the given inputs
		''' in native tensorflow </summary>
		''' <param name="inputs"> the inputs to run </param>
		''' <returns> the outputSchema from the native tensorflow wrapper </returns>
		Public Overridable Function runTfTensor(ByVal inputs As IDictionary(Of String, TF_Tensor)) As IDictionary(Of String, TF_Tensor)
			If graph Is Nothing Then
				Throw New System.InvalidOperationException("Graph not initialized.")
			End If


			If inputs.Count <> inputOrder.Count Then
				Throw New System.ArgumentException("Number of inputs specified do not match number of arrays specified.")
			End If

			If inputDataTypes Is Nothing Then
				inputDataTypes = New LinkedHashMap(Of String, TensorDataType)()
				For i As Integer = 0 To inputOrder.Count - 1
					inputDataTypes(inputOrder(i)) = TensorDataType.values()(TF_TensorType(inputs(inputOrder(i))))
				Next i
			End If

			For Each entry As KeyValuePair(Of String, org.bytedeco.tensorflow.TF_Tensor) In inputs.SetOfKeyValuePairs()
				Preconditions.checkNotNull(entry.Value,"Entry " & entry.Key & " was null!")
			Next entry

			'recast for adapting input
			inputs = recastInputs(inputs)


			If savedModelConfig IsNot Nothing Then
				Dim outputArrays As IDictionary(Of String, TF_Tensor) = New LinkedHashMap(Of String, TF_Tensor)()

				Dim opsByName As IDictionary(Of String, org.bytedeco.tensorflow.TF_Operation) = New Dictionary(Of String, org.bytedeco.tensorflow.TF_Operation)()
				Dim inputOut As New org.bytedeco.tensorflow.TF_Output(savedModelConfig.getSavedModelInputOrder().size())

				Dim inputTensors((savedModelConfig.getSavedModelInputOrder().size()) - 1) As TF_Tensor
				Dim i As Integer = 0
				Do While i < savedModelConfig.getSavedModelInputOrder().size()
					Dim name() As String = savedModelConfig.getSavedModelInputOrder().get(i).Split(":")
					Dim inputOp As org.bytedeco.tensorflow.TF_Operation = TF_GraphOperationByName(graph, name(0))
					opsByName(savedModelConfig.getSavedModelInputOrder().get(i)) = inputOp
					inputOut.position(i).oper(inputOp).index(If(name.Length > 1, Integer.Parse(name(1)), 0))
					Dim tfTensor As TF_Tensor = inputs(If(inputOrder IsNot Nothing AndAlso inputOrder.Count > 0, inputOrder(i), savedModelConfig.getSavedModelInputOrder().get(i)))
					inputTensors(i) = tfTensor
					i += 1
				Loop


				'reset the position of the pointer for execution
				inputOut.position(0)

				Dim outputOut As New org.bytedeco.tensorflow.TF_Output(savedModelConfig.getSaveModelOutputOrder().size())
				'only setup the output ops
				i = 0
				Do While i < savedModelConfig.getSaveModelOutputOrder().size()
					Dim name() As String = savedModelConfig.getSaveModelOutputOrder().get(i).Split(":")
					Dim outputOp As org.bytedeco.tensorflow.TF_Operation = TF_GraphOperationByName(graph, name(0))
					opsByName(savedModelConfig.getSaveModelOutputOrder().get(i)) = outputOp
					outputOut.position(i).oper(outputOp).index(If(name.Length > 1, Integer.Parse(name(1)), 0))
					i += 1
				Loop

				'reset the position of the pointer for execution
				outputOut.position(0)



				'these are references to the nd4j ndarrays wrapped for tensorflow
				Dim inputTensorsPointer As New PointerPointer(Of TF_Tensor)(inputTensors)
				'note that these are the result pointers
				'the result pointers are null, and will be populated automatically by the session run
				Dim outputTensorsPointer As New PointerPointer(Of TF_Tensor)(savedModelConfig.getSaveModelOutputOrder().size())

				Dim start As Long = System.nanoTime()
				TF_SessionRun(session, Nothing, inputOut, inputTensorsPointer, inputTensors.Length, outputOut, outputTensorsPointer, savedModelConfig.getSaveModelOutputOrder().size(), Nothing, 0, Nothing, status)
				Dim [end] As Long = System.nanoTime()
				Dim diff As Long = TimeUnit.NANOSECONDS.toMillis(([end] - start))
				log.debug("Session runtime: {} ms", diff)




				If TF_GetCode(status) <> TF_OK Then
					Throw New System.InvalidOperationException("ERROR: Unable to run session " & TF_Message(status).getString())
				Else
					For i As Integer = 0 To outputOrder.Count - 1
						outputArrays(If(outputOrder IsNot Nothing AndAlso outputOrder.Count > 0, outputOrder(i), savedModelConfig.getSaveModelOutputOrder().get(i))) = New TF_Tensor(outputTensorsPointer.get(i))
					Next i

				End If

				Return outputArrays

			Else
				Dim outputArrays As IDictionary(Of String, TF_Tensor) = New LinkedHashMap(Of String, TF_Tensor)()

				Dim opsByName As IDictionary(Of String, org.bytedeco.tensorflow.TF_Operation) = New Dictionary(Of String, org.bytedeco.tensorflow.TF_Operation)()
				Dim inputOut As New org.bytedeco.tensorflow.TF_Output(inputOrder.Count)

				Dim inputTensors(inputOrder.Count - 1) As TF_Tensor
				For i As Integer = 0 To inputOrder.Count - 1
					Dim name() As String = inputOrder(i).Split(":", True)
					Dim inputOp As org.bytedeco.tensorflow.TF_Operation = TF_GraphOperationByName(graph, name(0))
					opsByName(inputOrder(i)) = inputOp
					inputOut.position(i).oper(inputOp).index(If(name.Length > 1, Integer.Parse(name(1)), 0))
					Dim tf_tensor As TF_Tensor = inputs(inputOrder(i))

					inputTensors(i) = tf_tensor
				Next i


				'reset the position of the pointer for execution
				inputOut.position(0)

				Dim outputOut As New org.bytedeco.tensorflow.TF_Output(outputOrder.Count)
				'only setup the output ops
				For i As Integer = 0 To outputOrder.Count - 1
					Dim name() As String = outputOrder(i).Split(":", True)
					Dim outputOp As org.bytedeco.tensorflow.TF_Operation = TF_GraphOperationByName(graph, name(0))
					If outputOp Is Nothing Then
						Throw New System.ArgumentException("Illegal output found " & outputOrder(i) & " - no op found! Mis specified name perhaps?")
					End If

					opsByName(outputOrder(i)) = outputOp
					outputOut.position(i).oper(outputOp).index(If(name.Length > 1, Integer.Parse(name(1)), 0))
				Next i

				'reset the position of the pointer for execution
				outputOut.position(0)



				'these are references to the nd4j ndarrays wrapped for tensorflow
				Dim inputTensorsPointer As New PointerPointer(Of TF_Tensor)(inputTensors)
				'note that these are the result pointers
				'the result pointers are null, and will be populated automatically by the session run
				Dim outputTensorsPointer As New PointerPointer(Of TF_Tensor)(outputOrder.Count)

				Dim start As Long = System.nanoTime()
				TF_SessionRun(session, Nothing, inputOut, inputTensorsPointer, inputOrder.Count, outputOut, outputTensorsPointer, outputOrder.Count, Nothing, 0, Nothing, status)
				Dim [end] As Long = System.nanoTime()
				Dim diff As Long = TimeUnit.NANOSECONDS.toMillis(([end] - start))
				log.debug("Session runtime: {} ms", diff)






				If TF_GetCode(status) <> TF_OK Then
					Throw New System.InvalidOperationException("ERROR: Unable to run session " & TF_Message(status).getString())
				Else
					For i As Integer = 0 To outputOrder.Count - 1
						outputArrays(outputOrder(i)) = New TF_Tensor(outputTensorsPointer.get(i))
					Next i
				End If

				Return outputArrays
			End If
		End Function


		''' <summary>
		''' Returns a map of the output names
		''' to the ndarrays matching each output.
		''' 
		''' Note that <seealso cref="System.ArgumentException"/>
		''' will be thrown if there are any invalid states such as:
		''' the graph being null
		''' 
		''' 
		''' the inputs resolved from the graph do not match
		''' the inputs passed in
		''' 
		''' 
		''' </summary>
		''' <param name="inputs"> the inputs to use for each
		'''               <seealso cref="INDArray"/> </param>
		''' <returns> a map of the output names to the
		''' ndarrays matching each output specified in the graph </returns>

		Public Overridable Function run(ByVal inputs As IDictionary(Of String, INDArray)) As IDictionary(Of String, INDArray)
			If Not isTfWarmedUp AndAlso Not isTfWarmingUp Then
				isTfWarmingUp = True
				run(inputs)
				isTfWarmedUp = True
			End If
			Dim inputTensors As IDictionary(Of String, TF_Tensor) = New LinkedHashMap(Of String, TF_Tensor)()
			For Each input As KeyValuePair(Of String, INDArray) In inputs.SetOfKeyValuePairs()
				inputTensors(input.Key) = conversion.tensorFromNDArray(input.Value)
			Next input

			Dim outputTensors As IDictionary(Of String, TF_Tensor) = runTfTensor(inputTensors)
			Dim output As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			For Each outputTensor As KeyValuePair(Of String, TF_Tensor) In outputTensors.SetOfKeyValuePairs()
				output(outputTensor.Key) = conversion.ndArrayFromTensor(outputTensor.Value)
			Next outputTensor

			Return output
		End Function


		Private Sub initOptionsIfNeeded()
			'setup the status object to be used for all tensorflow calls
			If status Is Nothing Then
				status = TF_NewStatus()
			End If

			If options Is Nothing Then
				options = TF_NewSessionOptions()
				If sessionOptionsConfigProto IsNot Nothing Then
					Dim bytePointer As New BytePointer(sessionOptionsConfigProto.toByteArray())
					TF_SetConfig(options,bytePointer,bytePointer.getStringBytes().length,status)
					If TF_GetCode(status) <> TF_OK Then
						Throw New System.InvalidOperationException("ERROR: Unable to set value configuration:" & TF_Message(status).getString())
					End If
				End If
			End If
		End Sub

		Private Sub initSessionAndStatusIfNeeded(ByVal graphDef1 As org.tensorflow.framework.GraphDef)
			'infer the inputs and outputSchema for the graph
			Dim seenAsInput As ISet(Of String) = New LinkedHashSet(Of String)()
			Dim i As Integer = 0
			Do While i < graphDef1.getNodeCount()
				Dim node As NodeDef = graphDef1.getNode(i)
				Dim input As Integer = 0
				Do While input < node.getInputCount()
					seenAsInput.Add(node.getInput(input))
					input += 1
				Loop
				i += 1
			Loop

			If outputOrder Is Nothing Then
				outputOrder = New List(Of String)()
				log.trace("Attempting to automatically resolve tensorflow output names..")
				'find the nodes that were not inputs to any  nodes: these are the outputSchema
				i = 0
				Do While i < graphDef1.getNodeCount()
					If Not seenAsInput.Contains(graphDef1.getNode(i).getName()) AndAlso Not graphDef1.getNode(i).getOp().Equals("Placeholder") Then
						outputOrder.Add(graphDef1.getNode(i).getName())
					End If
					i += 1
				Loop

				'multiple names: purge any generated names from the output
				If outputOrder.Count > 1 Then
					Dim remove As ISet(Of String) = New HashSet(Of String)()
					For Each name As String In outputOrder
						If name.Contains("/") Then
							remove.Add(name)
						End If
					Next name

					outputOrder.RemoveAll(remove)
				End If
			End If


			'setup and configure the session, factoring
			'in the ConfigObject as needed
			If session Is Nothing Then
				initOptionsIfNeeded()
				session = TF_NewSession(graph, options, status)
				If TF_GetCode(status) <> TF_OK Then
					Throw New System.InvalidOperationException("ERROR: Unable to open session " & TF_Message(status).getString())
				End If

			End If

		End Sub

		Private Sub initSessionAndStatusIfNeeded(ByVal graphToUse() As SByte)
			If graphToUse Is Nothing Then
				'saved model configuration
				Return
			End If

			Try
				'use the protobuf api to load the graph definition and load the node metadata
				Dim graphDef1 As org.tensorflow.framework.GraphDef = org.tensorflow.framework.GraphDef.parseFrom(graphToUse)
				initSessionAndStatusIfNeeded(graphDef1)
			Catch e As InvalidProtocolBufferException
				log.error("",e)
			End Try
		End Sub


		''' <summary>
		''' Convert a json string written out
		''' by <seealso cref="org.nd4j.shade.protobuf.util.JsonFormat"/>
		''' to a <seealso cref="org.bytedeco.tensorflow.ConfigProto"/> </summary>
		''' <param name="json"> the json to read </param>
		''' <returns> the config proto to use </returns>
		Public Shared Function fromJson(ByVal json As String) As ConfigProto
			Dim builder As ConfigProto.Builder = ConfigProto.newBuilder()
			Try
				JsonFormat.parser().merge(json,builder)
				Dim build As ConfigProto = builder.build()
				Dim serialized As ByteString = build.toByteString()
				Dim binaryString() As SByte = serialized.toByteArray()
				Dim configProto As ConfigProto = ConfigProto.parseFrom(binaryString)
				Return configProto
			Catch e As Exception
				log.error("",e)
			End Try

			Return Nothing
		End Function


		''' <summary>
		''' Cast a tensor to another type using
		''' the tensorflow c api.
		''' This method loads a graph from the classpath from
		''' cast_graph/cast_(name of datatype lower case).pb
		''' which contains a simple protobuf file with a
		''' variant data type tensorflow input place holder
		''' named place holder and an output named cast_output. </summary>
		''' <param name="input">  the input data </param>
		''' <param name="from"> the input data type to cast from </param>
		''' <param name="to"> the output data type to </param>
		''' <returns> the casted tensor </returns>
		Public Shared Function castTensor(ByVal input As TF_Tensor, ByVal from As TensorDataType, ByVal [to] As TensorDataType) As TF_Tensor
			If from.Equals([to]) Then
				Return input
			End If

			Dim inputMap As IDictionary(Of String, TF_Tensor) = New Dictionary(Of String, TF_Tensor)()
			inputMap("input") = input
'JAVA TO VB CONVERTER NOTE: The variable graphRunner was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim graphRunner_Conflict As GraphRunner = getRunner(from,[to])
			Try
				Dim output As IDictionary(Of String, TF_Tensor) = graphRunner_Conflict.runTfTensor(inputMap)
				Return output("cast_output")

			Catch e As Exception
				Throw New System.InvalidOperationException("Unable to run graph",e)
			End Try
		End Function

		Private Shared Function getRunner(ByVal from As TensorDataType, ByVal [to] As TensorDataType) As GraphRunner
			Dim key As Pair(Of TensorDataType, TensorDataType) = Pair.of(from,[to])
			If Not recastGraphDefs.ContainsKey(key) Then
				Dim graphForDataType() As SByte = GraphRunner.graphForDataType(from,[to])
'JAVA TO VB CONVERTER NOTE: The variable graphRunner was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
				Dim graphRunner_Conflict As GraphRunner = GraphRunner.builder().graphBytes(graphForDataType).inputNames(java.util.Arrays.asList("input")).outputNames(java.util.Arrays.asList("cast_output")).build()

				recastGraphDefs(key) = graphRunner_Conflict
				Return graphRunner_Conflict
			End If

			Return recastGraphDefs(key)
		End Function


		Private Shared Function graphForDataType(ByVal from As TensorDataType, ByVal [to] As TensorDataType) As SByte()
			Dim classPathResource As New ClassPathResource("cast_graph/cast_" & TensorDataType.toPythonName(from) & "_" & TensorDataType.toPythonName([to]) & ".pb")
			Dim byteArrayOutputStream As New MemoryStream()
			Try
					Using [is] As Stream = classPathResource.InputStream
					IOUtils.copy([is], byteArrayOutputStream)
					End Using
			Catch e As IOException
				Throw New System.InvalidOperationException("Unable to read graph " & classPathResource.Filename,e)
			End Try

			Return byteArrayOutputStream.toByteArray()
		End Function

		''' <summary>
		''' Write out the session options used
		''' by this <seealso cref="org.nd4j.tensorflow.conversion.graphrunner.GraphRunner"/>
		''' a s a  json string using the
		''' <seealso cref="org.nd4j.shade.protobuf.util.JsonFormat"/> </summary>
		''' <returns> the session options as json (mainly for debugging) </returns>
		Public Overridable Function sessionOptionsToJson() As String
			If sessionOptionsConfigProto Is Nothing Then
				Return Nothing
			End If
			Try
				Return JsonFormat.printer().print(sessionOptionsConfigProto)
			Catch e As Exception
				log.error("",e)
			End Try

			Return Nothing
		End Function


		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If session IsNot Nothing AndAlso status IsNot Nothing Then
				TF_CloseSession(session, status)
				TF_DeleteSession(session,status)
			End If

			If status IsNot Nothing AndAlso TF_GetCode(status) <> TF_OK Then
				Throw New System.InvalidOperationException("ERROR: Unable to delete session " & TF_Message(status).getString())
			End If



			If status IsNot Nothing Then
				TF_DeleteStatus(status)
			End If
		End Sub
		Public Shared ReadOnly Property AlignedWithNd4j As ConfigProto
			Get
				Dim configProto As ConfigProto = ConfigProto.getDefaultInstance()
				Dim builder1 As ConfigProto.Builder = configProto.toBuilder().addDeviceFilters(TensorflowConversion.defaultDeviceForThread())
				Try
					'cuda
	'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					If Nd4j.Backend.GetType().FullName.ToLower().contains("jcu") Then
						builder1.setGpuOptions(org.tensorflow.framework.GPUOptions.newBuilder().setAllowGrowth(True).setPerProcessGpuMemoryFraction(0.5).build())
					'cpu
					Else
					End If
    
				Catch e As Exception
					log.error("",e)
				End Try
    
				Return builder1.build()
			End Get
		End Property



	End Class

End Namespace