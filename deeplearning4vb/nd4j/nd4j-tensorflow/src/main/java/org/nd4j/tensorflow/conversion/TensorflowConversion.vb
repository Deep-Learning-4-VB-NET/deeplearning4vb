Imports System
Imports System.Collections.Generic
Imports InvalidProtocolBufferException = org.nd4j.shade.protobuf.InvalidProtocolBufferException
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports SavedModelConfig = org.nd4j.tensorflow.conversion.graphrunner.SavedModelConfig
Imports MetaGraphDef = org.tensorflow.framework.MetaGraphDef
Imports SignatureDef = org.tensorflow.framework.SignatureDef
Imports TensorInfo = org.tensorflow.framework.TensorInfo
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

Namespace org.nd4j.tensorflow.conversion


	''' <summary>
	''' Interop between nd4j <seealso cref="INDArray"/>
	''' and <seealso cref="TF_Tensor"/>
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class TensorflowConversion

		'used for passing to tensorflow: this dummy de allocator
		'allows us to use nd4j buffers for memory management
		'rather than having them managed by tensorflow
		Private Shared calling As Deallocator_Pointer_long_Pointer
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared INSTANCE_Conflict As TensorflowConversion

		''' <summary>
		''' Get a singleton instance
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Instance As TensorflowConversion
			Get
				If INSTANCE_Conflict Is Nothing Then
					INSTANCE_Conflict = New TensorflowConversion()
				End If
				Return INSTANCE_Conflict
			End Get
		End Property


		Private Sub New()
			If calling Is Nothing Then
				calling = DummyDeAllocator.Instance
			End If

		End Sub


		''' <summary>
		''' Convert an <seealso cref="INDArray"/>
		''' to a <seealso cref="TF_Tensor"/>
		''' with zero copy.
		''' Uses a direct pointer to the underlying ndarray's
		''' data </summary>
		''' <param name="ndArray"> the ndarray to use </param>
		''' <returns> the equivalent <seealso cref="TF_Tensor"/> </returns>
		Public Overridable Function tensorFromNDArray(ByVal ndArray As INDArray) As TF_Tensor
		   If ndArray Is Nothing Then
			   Throw New System.ArgumentException("NDArray must not be null!")
		   End If
			'we infer data type from the ndarray.databuffer()
			'for now we throw an exception
			If ndArray.data() Is Nothing Then
			   Throw New System.ArgumentException("Unable to infer data type from null databuffer")
			End If

			If ndArray.View OrElse ndArray.ordering() <> "c"c Then
				ndArray = ndArray.dup("c"c)
			End If


			Dim ndShape() As Long = ndArray.shape()
			Dim tfShape(ndShape.Length - 1) As Long
			Array.Copy(ndShape, 0, tfShape, 0, ndShape.Length)

			Dim type As Integer
			Dim data As DataBuffer = ndArray.data()
			Dim dataType As DataType = data.dataType()
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					type = DT_DOUBLE
				Case DataType.InnerEnum.FLOAT
					type = DT_FLOAT
				Case DataType.InnerEnum.INT
					type = DT_INT32
				Case DataType.InnerEnum.HALF
					type = DT_HALF
				Case DataType.InnerEnum.COMPRESSED
					Dim compressedData As CompressedDataBuffer = DirectCast(data, CompressedDataBuffer)
					Dim desc As CompressionDescriptor = compressedData.getCompressionDescriptor()
					Dim algo As String = desc.getCompressionAlgorithm()
					Select Case algo
						Case "FLOAT16"
							type = DT_HALF
						Case "INT8"
							type = DT_INT8
						Case "UINT8"
							type = DT_UINT8
						Case "INT16"
							type = DT_INT16
						Case "UINT16"
							type = DT_UINT16
						Case Else
							Throw New System.ArgumentException("Unsupported compression algorithm: " & algo)
					End Select
				Case DataType.InnerEnum.SHORT
					type = DT_INT16
				Case DataType.InnerEnum.LONG
					type = DT_INT64
				Case DataType.InnerEnum.UTF8
					type = DT_STRING
				Case DataType.InnerEnum.BYTE
					type = DT_INT8
				Case DataType.InnerEnum.UBYTE
					type = DT_UINT8
				Case DataType.InnerEnum.UINT16
					type = DT_UINT16
				Case DataType.InnerEnum.UINT32
					type = DT_UINT32
				Case DataType.InnerEnum.UINT64
					type = DT_UINT64
				Case DataType.InnerEnum.BFLOAT16
					type = DT_BFLOAT16
				Case DataType.InnerEnum.BOOL
					type = DT_BOOL
				Case Else
					Throw New System.ArgumentException("Unsupported data type: " & dataType)
			End Select

			Try
				Nd4j.AffinityManager.ensureLocation(ndArray, AffinityManager.Location.HOST)
			Catch e As Exception
				' ND4J won't let us access compressed data in GPU memory, so we'll leave TensorFlow do the conversion instead
				ndArray.getDouble(0) ' forces decompression and data copy to host
				data = ndArray.data()
				dataType = data.dataType()
				Select Case dataType.innerEnumValue
					Case DataType.InnerEnum.DOUBLE
						type = DT_DOUBLE
					Case DataType.InnerEnum.FLOAT
						type = DT_FLOAT
					Case DataType.InnerEnum.INT
						type = DT_INT32
					Case DataType.InnerEnum.LONG
						type = DT_INT64
					Case DataType.InnerEnum.UTF8
						type = DT_STRING
					Case Else
						Throw New System.ArgumentException("Unsupported data type: " & dataType)
				End Select
			End Try


			Dim longPointer As New LongPointer(tfShape)
			Dim tf_tensor As TF_Tensor = Nothing

			If type = DT_STRING Then
				Dim size As Long = 0
				Dim length As Long = ndArray.length()
				Dim strings(CInt(length) - 1) As BytePointer
				For i As Integer = 0 To length - 1
					strings(i) = New BytePointer(ndArray.getString(i))
					size += TF_StringEncodedSize(strings(i).capacity())
				Next i
				tf_tensor = TF_AllocateTensor(type, longPointer, tfShape.Length, 8 * length + size)

				Dim offset As Long = 0
				Dim tf_data As BytePointer = (New BytePointer(TF_TensorData(tf_tensor))).capacity(TF_TensorByteSize(tf_tensor))
				Dim status As TF_Status = TF_NewStatus()
				For i As Integer = 0 To length - 1
					tf_data.position(8 * i).putLong(offset)
					offset += TF_StringEncode(strings(i), strings(i).capacity() - 1, tf_data.position(8 * length + offset), tf_data.capacity() - tf_data.position(), status)
					If TF_GetCode(status) <> TF_OK Then
						Throw New System.InvalidOperationException("ERROR: Unable to convert tensor " & TF_Message(status).getString())
					End If
				Next i
				TF_DeleteStatus(status)
			Else
				tf_tensor = TF_NewTensor(type, longPointer, tfShape.Length, data.pointer(), data.length() * data.ElementSize, calling,Nothing)
			End If

			Return tf_tensor

		End Function

		''' <summary>
		''' Convert a <seealso cref="INDArray"/>
		''' to a <seealso cref="TF_Tensor"/>
		'''  using zero copy.
		'''  It will use the underlying
		'''  pointer with in nd4j. </summary>
		''' <param name="tensor"> the tensor to use
		''' @return </param>
		Public Overridable Function ndArrayFromTensor(ByVal tensor As TF_Tensor) As INDArray
			Dim rank As Integer = TF_NumDims(tensor)

			Dim ndShape() As Integer
			If rank = 0 Then
				' scalar
				ndShape = New Integer() { 1 }
			Else
				ndShape = New Integer(rank - 1){}
				For i As Integer = 0 To ndShape.Length - 1
					ndShape(i) = CInt(Math.Truncate(TF_Dim(tensor,i)))
				Next i
			End If

			Dim tfType As Integer = TF_TensorType(tensor)
			Dim nd4jType As DataType = typeFor(tfType)

			Dim length As Integer = ArrayUtil.prod(ndShape)
			Dim array As INDArray
			If nd4jType = DataType.UTF8 Then
				Dim strings(length - 1) As String
				Dim data As BytePointer = (New BytePointer(TF_TensorData(tensor))).capacity(TF_TensorByteSize(tensor))
				Dim str As New BytePointer(DirectCast(Nothing, Pointer))
				Dim size As New SizeTPointer(1)
				Dim status As TF_Status = TF_NewStatus()
				For i As Integer = 0 To length - 1
					Dim offset As Long = data.position(8 * i).getLong()
					TF_StringDecode(data.position(8 * length + offset), data.capacity() - data.position(), str, size, status)
					If TF_GetCode(status) <> TF_OK Then
						Throw New System.InvalidOperationException("ERROR: Unable to convert tensor " & TF_Message(status).getString())
					End If
					strings(i) = str.position(0).capacity(size.get()).getString()
				Next i
				TF_DeleteStatus(status)
				array = Nd4j.create(strings)
			Else
				Dim pointer As Pointer = TF_TensorData(tensor).capacity(length)
				Dim indexer As Indexer = indexerForType(nd4jType,pointer)
				Dim d As DataBuffer = Nd4j.createBuffer(indexer.pointer(),nd4jType,length,indexer)
				array = Nd4j.create(d,ndShape)
			End If
			' we don't need this in this case. Device memory will be updated right in the constructor
			'Nd4j.getAffinityManager().tagLocation(array, AffinityManager.Location.HOST);
			Return array
		End Function




		Private Function indexerForType(ByVal type As DataType, ByVal pointer As Pointer) As Indexer
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return DoubleIndexer.create(New DoublePointer(pointer))
				Case DataType.InnerEnum.FLOAT
					Return FloatIndexer.create(New FloatPointer(pointer))
				Case DataType.InnerEnum.INT
					Return IntIndexer.create(New IntPointer(pointer))
				Case DataType.InnerEnum.LONG
					Return LongIndexer.create(New LongPointer(pointer))
				Case DataType.InnerEnum.SHORT
					Return ShortIndexer.create(New ShortPointer(pointer))
				Case DataType.InnerEnum.BYTE
					Return ByteIndexer.create(New BytePointer(pointer))
				Case DataType.InnerEnum.UBYTE
					Return UByteIndexer.create(New BytePointer(pointer))
				Case DataType.InnerEnum.UINT16
					Return UShortIndexer.create(New ShortPointer(pointer))
				Case DataType.InnerEnum.UINT32
					Return UIntIndexer.create(New IntPointer(pointer))
				Case DataType.InnerEnum.UINT64
					Return ULongIndexer.create(New LongPointer(pointer))
				Case DataType.InnerEnum.BFLOAT16
					Return Bfloat16Indexer.create(New ShortPointer(pointer))
				Case DataType.InnerEnum.HALF
					Return HalfIndexer.create(New ShortPointer(pointer))
				Case DataType.InnerEnum.BOOL
					Return BooleanIndexer.create(New BooleanPointer(pointer))
				Case Else
					Throw New System.ArgumentException("Illegal type " & type)
			End Select
		End Function

		Private Function typeFor(ByVal tensorflowType As Integer) As DataType
			Select Case tensorflowType
				Case DT_DOUBLE
					Return DataType.DOUBLE
				Case DT_FLOAT
					Return DataType.FLOAT
				Case DT_HALF
					Return DataType.HALF
				Case DT_INT16
					Return DataType.SHORT
				Case DT_INT32
					Return DataType.INT
				Case DT_INT64
					Return DataType.LONG
				Case DT_STRING
					Return DataType.UTF8
				Case DT_INT8
					Return DataType.BYTE
				Case DT_UINT8
					Return DataType.UBYTE
				Case DT_UINT16
					Return DataType.UINT16
				Case DT_UINT32
					Return DataType.UINT32
				Case DT_UINT64
					Return DataType.UINT64
				Case DT_BFLOAT16
					Return DataType.BFLOAT16
				Case DT_BOOL
					Return DataType.BOOL
				Case Else
					Throw New System.ArgumentException("Illegal type " & tensorflowType)
			End Select
		End Function

		''' <summary>
		''' Get an initialized <seealso cref="TF_Graph"/>
		''' based on the passed in file
		''' (the file must be a binary protobuf/pb file)
		''' The graph will be modified to be associated
		''' with the device associated with this current thread.
		''' 
		''' Depending on the active <seealso cref="Nd4j.getBackend()"/>
		''' the device will either be the gpu pinned to the current thread
		''' or the cpu </summary>
		''' <param name="filePath"> the path to the file to read </param>
		''' <returns> the initialized graph </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TF_Graph loadGraph(String filePath, TF_Status status) throws java.io.IOException
		Public Overridable Function loadGraph(ByVal filePath As String, ByVal status As TF_Status) As TF_Graph
			Dim bytes() As SByte = Files.readAllBytes(Paths.get(filePath))
			Return loadGraph(bytes, status)
		End Function

		''' <summary>
		''' Infers the device for the given thread
		''' based on the <seealso cref="Nd4j.getAffinityManager()"/>
		''' Usually, this will either be a gpu or cpu
		''' reserved for the current device.
		''' You can think of the "current thread"
		''' as a worker. This is mainly useful with multiple gpus
		''' @return
		''' </summary>
		Public Shared Function defaultDeviceForThread() As String
			Dim deviceForThread As Integer? = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Dim deviceName As String = Nothing
			'gpu
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If Nd4j.Backend.GetType().FullName.Contains("JCublasBackend") Then
				deviceName = "/device:gpu:" & deviceForThread
			Else
				deviceName = "/device:cpu:" & deviceForThread
			End If


			Return deviceName
		End Function



		''' <summary>
		''' Get an initialized <seealso cref="TF_Graph"/>
		''' based on the passed in byte array content
		''' (the content must be a binary protobuf/pb file)
		''' The graph will be modified to be associated
		''' with the device associated with this current thread.
		''' 
		''' Depending on the active <seealso cref="Nd4j.getBackend()"/>
		''' the device will either be the gpu pinned to the current thread
		''' or the content </summary>
		''' <param name="content"> the path to the file to read </param>
		''' <returns> the initialized graph </returns>
		''' <exception cref="IOException"> </exception>

		Public Overridable Function loadGraph(ByVal content() As SByte, ByVal status As TF_Status) As TF_Graph
			Dim toLoad() As SByte = content
			Dim graph_def As TF_Buffer = TF_NewBufferFromString(New BytePointer(toLoad), content.Length)
			Dim graphC As TF_Graph = TF_NewGraph()
			Dim opts As TF_ImportGraphDefOptions = TF_NewImportGraphDefOptions()
			TF_GraphImportGraphDef(graphC, graph_def, opts, status)
			If TF_GetCode(status) <> TF_OK Then
				Throw New System.InvalidOperationException("ERROR: Unable to import graph " & TF_Message(status).getString())
			End If


			TF_DeleteImportGraphDefOptions(opts)

			Return graphC
		End Function

		''' <summary>
		''' Load a session based on the saved model </summary>
		''' <param name="savedModelConfig"> the configuration for the saved model </param>
		''' <param name="options"> the session options to use </param>
		''' <param name="runOptions"> the run configuration to use </param>
		''' <param name="graph"> the tf graph to use </param>
		''' <param name="inputsMap"> the input map </param>
		''' <param name="outputsMap"> the output names </param>
		''' <param name="status">  the status object to use for verifying the results
		''' @return </param>
		Public Overridable Function loadSavedModel(ByVal savedModelConfig As SavedModelConfig, ByVal options As TF_SessionOptions, ByVal runOptions As TF_Buffer, ByVal graph As TF_Graph, ByVal inputsMap As IDictionary(Of String, String), ByVal outputsMap As IDictionary(Of String, String), ByVal status As TF_Status) As TF_Session
			Dim metaGraph As TF_Buffer = TF_Buffer.newBuffer()
			Dim session As TF_Session = TF_LoadSessionFromSavedModel(options, runOptions, New BytePointer(savedModelConfig.getSavedModelPath()), New BytePointer(savedModelConfig.getModelTag()), 1, graph, metaGraph, status)
			If TF_GetCode(status) <> TF_OK Then
				Throw New System.InvalidOperationException("ERROR: Unable to import model " & TF_Message(status).getString())
			End If

			Dim metaGraphDef As MetaGraphDef
			Try
				metaGraphDef = MetaGraphDef.parseFrom(metaGraph.data().capacity(metaGraph.length()).asByteBuffer())
			Catch ex As InvalidProtocolBufferException
				Throw New System.InvalidOperationException("ERROR: Unable to import model " & ex)
			End Try
			Dim signatureDefMap As IDictionary(Of String, SignatureDef) = metaGraphDef.getSignatureDefMap()
			Dim signatureDef As SignatureDef = signatureDefMap(savedModelConfig.getSignatureKey())

			Dim inputs As IDictionary(Of String, TensorInfo) = signatureDef.getInputsMap()
			For Each e As KeyValuePair(Of String, TensorInfo) In inputs.SetOfKeyValuePairs()
				inputsMap(e.Key) = e.Value.getName()
			Next e

			Dim outputs As IDictionary(Of String, TensorInfo) = signatureDef.getOutputsMap()
			For Each e As KeyValuePair(Of String, TensorInfo) In outputs.SetOfKeyValuePairs()
				outputsMap(e.Key) = e.Value.getName()
			Next e

			Return session
		End Function
	End Class

End Namespace