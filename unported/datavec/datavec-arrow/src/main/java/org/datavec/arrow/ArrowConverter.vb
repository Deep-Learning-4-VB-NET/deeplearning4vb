Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BufferAllocator = org.apache.arrow.memory.BufferAllocator
Imports RootAllocator = org.apache.arrow.memory.RootAllocator
Imports org.apache.arrow.vector
Imports Dictionary = org.apache.arrow.vector.dictionary.Dictionary
Imports DictionaryProvider = org.apache.arrow.vector.dictionary.DictionaryProvider
Imports ArrowFileReader = org.apache.arrow.vector.ipc.ArrowFileReader
Imports ArrowFileWriter = org.apache.arrow.vector.ipc.ArrowFileWriter
Imports SeekableReadChannel = org.apache.arrow.vector.ipc.SeekableReadChannel
Imports ArrowRecordBatch = org.apache.arrow.vector.ipc.message.ArrowRecordBatch
Imports DateUnit = org.apache.arrow.vector.types.DateUnit
Imports FloatingPointPrecision = org.apache.arrow.vector.types.FloatingPointPrecision
Imports ArrowType = org.apache.arrow.vector.types.pojo.ArrowType
Imports DictionaryEncoding = org.apache.arrow.vector.types.pojo.DictionaryEncoding
Imports Field = org.apache.arrow.vector.types.pojo.Field
Imports FieldType = org.apache.arrow.vector.types.pojo.FieldType
Imports ByteArrayReadableSeekableByteChannel = org.apache.arrow.vector.util.ByteArrayReadableSeekableByteChannel
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.metadata
Imports Schema = org.datavec.api.transform.schema.Schema
Imports TypeConversion = org.datavec.api.transform.schema.conversion.TypeConversion
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports org.datavec.api.writable
Imports ArrowWritableRecordBatch = org.datavec.arrow.recordreader.ArrowWritableRecordBatch
Imports ArrowWritableRecordTimeSeriesBatch = org.datavec.arrow.recordreader.ArrowWritableRecordTimeSeriesBatch
Imports org.nd4j.common.primitives
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalArgumentException = org.nd4j.linalg.exception.ND4JIllegalArgumentException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BinarySerde = org.nd4j.serde.binary.BinarySerde

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

Namespace org.datavec.arrow



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ArrowConverter
	Public Class ArrowConverter




		''' <summary>
		''' Create an ndarray from a matrix.
		''' The included batch must be all the same number of rows in order
		''' to work. The reason for this is <seealso cref="INDArray"/> must be all the same dimensions.
		''' Note that the input columns must also be numerical. If they aren't numerical already,
		''' consider using an <seealso cref="org.datavec.api.transform.TransformProcess"/> to transform the data
		''' output from <seealso cref="org.datavec.arrow.recordreader.ArrowRecordReader"/> in to the proper format
		''' for usage with this method for direct conversion.
		''' </summary>
		''' <param name="arrowWritableRecordBatch"> the incoming batch. This is typically output from
		'''                                 an <seealso cref="org.datavec.arrow.recordreader.ArrowRecordReader"/> </param>
		''' <returns> an <seealso cref="INDArray"/> representative of the input data </returns>
		Public Shared Function toArray(ByVal arrowWritableRecordBatch As ArrowWritableRecordTimeSeriesBatch) As INDArray
			Return RecordConverter.toTensor(arrowWritableRecordBatch)
		End Function




		''' <summary>
		''' Create an ndarray from a matrix.
		''' The included batch must be all the same number of rows in order
		''' to work. The reason for this is <seealso cref="INDArray"/> must be all the same dimensions.
		''' Note that the input columns must also be numerical. If they aren't numerical already,
		''' consider using an <seealso cref="org.datavec.api.transform.TransformProcess"/> to transform the data
		''' output from <seealso cref="org.datavec.arrow.recordreader.ArrowRecordReader"/> in to the proper format
		''' for usage with this method for direct conversion.
		''' </summary>
		''' <param name="arrowWritableRecordBatch"> the incoming batch. This is typically output from
		'''                                 an <seealso cref="org.datavec.arrow.recordreader.ArrowRecordReader"/> </param>
		''' <returns> an <seealso cref="INDArray"/> representative of the input data </returns>
		Public Shared Function toArray(ByVal arrowWritableRecordBatch As ArrowWritableRecordBatch) As INDArray
			Dim columnVectors As IList(Of FieldVector) = arrowWritableRecordBatch.getList()
			Dim schema As Schema = arrowWritableRecordBatch.getSchema()
			Dim i As Integer = 0
			Do While i < schema.numColumns()
				Select Case schema.getType(i)
					Case Integer?
					Case Single?
					Case Double?
					Case Long?
					Case NDArray
					Case Else
						Throw New ND4JIllegalArgumentException("Illegal data type found for column " & schema.getName(i) & " of type " & schema.getType(i))
				End Select
				i += 1
			Loop


			Dim rows As Integer = arrowWritableRecordBatch.getList().get(0).getValueCount()

			If schema.numColumns() = 1 AndAlso schema.getMetaData(0).ColumnType = ColumnType.NDArray Then
				Dim toConcat(rows - 1) As INDArray
				Dim valueVectors As VarBinaryVector = CType(arrowWritableRecordBatch.getList().get(0), VarBinaryVector)
				For i As Integer = 0 To rows - 1
					Dim bytes() As SByte = valueVectors.get(i)
					Dim direct As ByteBuffer = ByteBuffer.allocateDirect(bytes.Length)
					direct.put(bytes)
					Dim fromTensor As INDArray = BinarySerde.toArray(direct)
					toConcat(i) = fromTensor
				Next i

				Return Nd4j.concat(0,toConcat)

			End If

			Dim cols As Integer = schema.numColumns()
			Dim arr As INDArray = Nd4j.create(rows,cols)
			For i As Integer = 0 To cols - 1
				Dim put As INDArray = ArrowConverter.convertArrowVector(columnVectors(i),schema.getType(i))
				Select Case arr.data().dataType()
					Case FLOAT
						arr.putColumn(i,Nd4j.create(put.data().asFloat()).reshape(ChrW(rows), 1))
					Case [DOUBLE]
						arr.putColumn(i,Nd4j.create(put.data().asDouble()).reshape(ChrW(rows), 1))
				End Select

			Next i

			Return arr
		End Function

		''' <summary>
		''' Convert a field vector to a column vector </summary>
		''' <param name="fieldVector"> the field vector to convert </param>
		''' <param name="type"> the type of the column vector </param>
		''' <returns> the converted ndarray </returns>
		Public Shared Function convertArrowVector(ByVal fieldVector As FieldVector, ByVal type As ColumnType) As INDArray
			Dim buffer As DataBuffer = Nothing
			Dim cols As Integer = fieldVector.getValueCount()
			Dim direct As ByteBuffer = ByteBuffer.allocateDirect(CInt(fieldVector.getDataBuffer().capacity()))
			direct.order(ByteOrder.nativeOrder())
			fieldVector.getDataBuffer().getBytes(0,direct)
			Dim buffer1 As Buffer = CType(direct, Buffer)
			buffer1.rewind()
			Select Case type.innerEnumValue
				Case Integer?
					buffer = Nd4j.createBuffer(direct, DataType.INT,cols,0)
				Case Single?
					buffer = Nd4j.createBuffer(direct, DataType.FLOAT,cols)
				Case Double?
					buffer = Nd4j.createBuffer(direct, DataType.DOUBLE,cols)
				Case Long?
					buffer = Nd4j.createBuffer(direct, DataType.LONG,cols)
			End Select

			Return Nd4j.create(buffer,New Integer() {cols, 1})
		End Function


		''' <summary>
		''' Convert an <seealso cref="INDArray"/>
		''' to a list of column vectors or a singleton
		''' list when either a row vector or a column vector </summary>
		''' <param name="from"> the input array </param>
		''' <param name="name"> the name of the vector </param>
		''' <param name="type"> the type of the vector </param>
		''' <param name="bufferAllocator"> the allocator to use </param>
		''' <returns> the list of field vectors </returns>
		Public Shared Function convertToArrowVector(ByVal from As INDArray, ByVal name As IList(Of String), ByVal type As ColumnType, ByVal bufferAllocator As BufferAllocator) As IList(Of FieldVector)
			Dim ret As IList(Of FieldVector) = New List(Of FieldVector)()
			If from.Vector Then
				Dim cols As Long = from.length()
				Select Case type.innerEnumValue
					Case Double?
						Dim fromData() As Double = If(from.View, from.dup().data().asDouble(), from.data().asDouble())
						ret.Add(vectorFor(bufferAllocator,name(0),fromData))
					Case Single?
						Dim fromDataFloat() As Single = If(from.View, from.dup().data().asFloat(), from.data().asFloat())
						ret.Add(vectorFor(bufferAllocator,name(0),fromDataFloat))
					Case Integer?
						Dim fromDataInt() As Integer = If(from.View, from.dup().data().asInt(), from.data().asInt())
						ret.Add(vectorFor(bufferAllocator,name(0),fromDataInt))
					Case Else
						Throw New System.ArgumentException("Illegal type " & type)
				End Select

			Else
				Dim cols As Long = from.size(1)
				For i As Integer = 0 To cols - 1
					Dim column As INDArray = from.getColumn(i)

					Select Case type.innerEnumValue
						Case Double?
							Dim fromData() As Double = If(column.View, column.dup().data().asDouble(), from.data().asDouble())
							ret.Add(vectorFor(bufferAllocator,name(i),fromData))
						Case Single?
							Dim fromDataFloat() As Single = If(column.View, column.dup().data().asFloat(), from.data().asFloat())
							ret.Add(vectorFor(bufferAllocator,name(i),fromDataFloat))
						Case Integer?
							Dim fromDataInt() As Integer = If(column.View, column.dup().data().asInt(), from.data().asInt())
							ret.Add(vectorFor(bufferAllocator,name(i),fromDataInt))
						Case Else
							Throw New System.ArgumentException("Illegal type " & type)
					End Select
				Next i
			End If


			Return ret
		End Function



		''' <summary>
		''' Write the records to the given output stream </summary>
		''' <param name="recordBatch"> the record batch to write </param>
		''' <param name="inputSchema"> the input schema </param>
		''' <param name="outputStream"> the output stream to write to </param>
		Public Shared Sub writeRecordBatchTo(ByVal recordBatch As IList(Of IList(Of Writable)), ByVal inputSchema As Schema, ByVal outputStream As Stream)
			Dim bufferAllocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			writeRecordBatchTo(bufferAllocator,recordBatch,inputSchema,outputStream)
		End Sub

		''' <summary>
		''' Write the records to the given output stream </summary>
		''' <param name="recordBatch"> the record batch to write </param>
		''' <param name="inputSchema"> the input schema </param>
		''' <param name="outputStream"> the output stream to write to </param>
		Public Shared Sub writeRecordBatchTo(ByVal bufferAllocator As BufferAllocator, ByVal recordBatch As IList(Of IList(Of Writable)), ByVal inputSchema As Schema, ByVal outputStream As Stream)
			If Not (TypeOf recordBatch Is ArrowWritableRecordBatch) Then
				Dim convertedSchema As val = toArrowSchema(inputSchema)
				Dim columns As IList(Of FieldVector) = toArrowColumns(bufferAllocator,inputSchema,recordBatch)
				Try
					Dim root As New VectorSchemaRoot(convertedSchema,columns,recordBatch.Count)

					Dim writer As New ArrowFileWriter(root, providerForVectors(columns,convertedSchema.getFields()), newChannel(outputStream))
					writer.start()
					writer.writeBatch()
					writer.end()


				Catch e As IOException
					Throw New System.InvalidOperationException(e)
				End Try

			Else
				Dim convertedSchema As val = toArrowSchema(inputSchema)
				Dim pair As val = toArrowColumns(bufferAllocator,inputSchema,recordBatch)
				Try
					Dim root As New VectorSchemaRoot(convertedSchema,pair,recordBatch.Count)

					Dim writer As New ArrowFileWriter(root, providerForVectors(pair,convertedSchema.getFields()), newChannel(outputStream))
					writer.start()
					writer.writeBatch()
					writer.end()


				Catch e As IOException
					Throw New System.InvalidOperationException(e)
				End Try
			End If

		End Sub


		''' <summary>
		''' Convert the input field vectors (the input data) and
		''' the given schema to a proper list of writables. </summary>
		''' <param name="fieldVectors"> the field vectors to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <param name="timeSeriesLength"> the length of the time series </param>
		''' <returns> the equivalent datavec batch given the input data </returns>
		Public Shared Function toArrowWritablesTimeSeries(ByVal fieldVectors As IList(Of FieldVector), ByVal schema As Schema, ByVal timeSeriesLength As Integer) As IList(Of IList(Of IList(Of Writable)))
			Dim arrowWritableRecordBatch As New ArrowWritableRecordTimeSeriesBatch(fieldVectors,schema,timeSeriesLength)
			Return arrowWritableRecordBatch
		End Function


		''' <summary>
		''' Convert the input field vectors (the input data) and
		''' the given schema to a proper list of writables. </summary>
		''' <param name="fieldVectors"> the field vectors to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <returns> the equivalent datavec batch given the input data </returns>
		Public Shared Function toArrowWritables(ByVal fieldVectors As IList(Of FieldVector), ByVal schema As Schema) As ArrowWritableRecordBatch
			Dim arrowWritableRecordBatch As New ArrowWritableRecordBatch(fieldVectors,schema)
			Return arrowWritableRecordBatch
		End Function

		''' <summary>
		''' Return a singular record based on the converted
		''' writables result. </summary>
		''' <param name="fieldVectors"> the field vectors to use </param>
		''' <param name="schema"> the schema to use for input
		''' @return </param>
		Public Shared Function toArrowWritablesSingle(ByVal fieldVectors As IList(Of FieldVector), ByVal schema As Schema) As IList(Of Writable)
			Return toArrowWritables(fieldVectors,schema)(0)
		End Function


		''' <summary>
		''' Read a datavec schema and record set
		''' from the given arrow file. </summary>
		''' <param name="input"> the input to read </param>
		''' <returns> the associated datavec schema and record </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.datavec.api.transform.schema.Schema,org.datavec.arrow.recordreader.ArrowWritableRecordBatch> readFromFile(java.io.FileInputStream input) throws java.io.IOException
		Public Shared Function readFromFile(ByVal input As FileStream) As Pair(Of Schema, ArrowWritableRecordBatch)
			Dim allocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			Dim retSchema As Schema = Nothing
			Dim ret As ArrowWritableRecordBatch = Nothing
			Dim channel As New SeekableReadChannel(input.getChannel())
			Dim reader As New ArrowFileReader(channel, allocator)
			reader.loadNextBatch()
			retSchema = toDatavecSchema(reader.getVectorSchemaRoot().getSchema())
			'load the batch
			Dim unloader As New VectorUnloader(reader.getVectorSchemaRoot())
			Dim vectorLoader As New VectorLoader(reader.getVectorSchemaRoot())
			Dim recordBatch As ArrowRecordBatch = unloader.getRecordBatch()

			vectorLoader.load(recordBatch)
			ret = asDataVecBatch(recordBatch,retSchema,reader.getVectorSchemaRoot())
			ret.setUnloader(unloader)

			Return Pair.of(retSchema,ret)

		End Function

		''' <summary>
		''' Read a datavec schema and record set
		''' from the given arrow file. </summary>
		''' <param name="input"> the input to read </param>
		''' <returns> the associated datavec schema and record </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.datavec.api.transform.schema.Schema,org.datavec.arrow.recordreader.ArrowWritableRecordBatch> readFromFile(java.io.File input) throws java.io.IOException
		Public Shared Function readFromFile(ByVal input As File) As Pair(Of Schema, ArrowWritableRecordBatch)
			Return readFromFile(New FileStream(input, FileMode.Open, FileAccess.Read))
		End Function

		''' <summary>
		''' Read a datavec schema and record set
		''' from the given bytes (usually expected to be an arrow format file) </summary>
		''' <param name="input"> the input to read </param>
		''' <returns> the associated datavec schema and record </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.datavec.api.transform.schema.Schema,org.datavec.arrow.recordreader.ArrowWritableRecordBatch> readFromBytes(byte[] input) throws java.io.IOException
		Public Shared Function readFromBytes(ByVal input() As SByte) As Pair(Of Schema, ArrowWritableRecordBatch)
			Dim allocator As BufferAllocator = New RootAllocator(Long.MaxValue)
			Dim retSchema As Schema = Nothing
			Dim ret As ArrowWritableRecordBatch = Nothing
			Dim channel As New SeekableReadChannel(New ByteArrayReadableSeekableByteChannel(input))
			Dim reader As New ArrowFileReader(channel, allocator)
			reader.loadNextBatch()
			retSchema = toDatavecSchema(reader.getVectorSchemaRoot().getSchema())
			'load the batch
			Dim unloader As New VectorUnloader(reader.getVectorSchemaRoot())
			Dim vectorLoader As New VectorLoader(reader.getVectorSchemaRoot())
			Dim recordBatch As ArrowRecordBatch = unloader.getRecordBatch()

			vectorLoader.load(recordBatch)
			ret = asDataVecBatch(recordBatch,retSchema,reader.getVectorSchemaRoot())
			ret.setUnloader(unloader)

			Return Pair.of(retSchema,ret)

		End Function

		''' <summary>
		''' Convert a data vec <seealso cref="Schema"/>
		''' to an arrow <seealso cref="org.apache.arrow.vector.types.pojo.Schema"/> </summary>
		''' <param name="schema"> the input schema </param>
		''' <returns> the schema for arrow </returns>
		Public Shared Function toArrowSchema(ByVal schema As Schema) As org.apache.arrow.vector.types.pojo.Schema
			Dim fields As IList(Of Field) = New List(Of Field)(schema.numColumns())
			Dim i As Integer = 0
			Do While i < schema.numColumns()
				fields.Add(getFieldForColumn(schema.getName(i),schema.getType(i)))
				i += 1
			Loop

			Return New org.apache.arrow.vector.types.pojo.Schema(fields)
		End Function

		''' <summary>
		''' Convert an <seealso cref="org.apache.arrow.vector.types.pojo.Schema"/>
		''' to a datavec <seealso cref="Schema"/> </summary>
		''' <param name="schema"> the input arrow schema </param>
		''' <returns> the equivalent datavec schema </returns>
		Public Shared Function toDatavecSchema(ByVal schema As org.apache.arrow.vector.types.pojo.Schema) As Schema
			Dim schemaBuilder As New Schema.Builder()
			Dim i As Integer = 0
			Do While i < schema.getFields().size()
				schemaBuilder.addColumn(metaDataFromField(schema.getFields().get(i)))
				i += 1
			Loop
			Return schemaBuilder.build()
		End Function




		''' <summary>
		''' Shortcut method for returning a field
		''' given an arrow type and name
		''' with no sub fields </summary>
		''' <param name="name"> the name of the field </param>
		''' <param name="arrowType"> the arrow type of the field </param>
		''' <returns> the resulting field </returns>
		Public Shared Function field(ByVal name As String, ByVal arrowType As ArrowType) As Field
			Return New Field(name,FieldType.nullable(arrowType), New List(Of Field)())
		End Function



		''' <summary>
		''' Create a field given the input <seealso cref="ColumnType"/>
		''' and name </summary>
		''' <param name="name"> the name of the field </param>
		''' <param name="columnType"> the column type to add
		''' @return </param>
		Public Shared Function getFieldForColumn(ByVal name As String, ByVal columnType As ColumnType) As Field
			Select Case columnType.innerEnumValue
				Case Long?
					Return field(name,New ArrowType.Int(64,False))
				Case Integer?
					Return field(name,New ArrowType.Int(32,False))
				Case Double?
					Return field(name,New ArrowType.FloatingPoint(FloatingPointPrecision.DOUBLE))
				Case Single?
					Return field(name,New ArrowType.FloatingPoint(FloatingPointPrecision.SINGLE))
				Case Boolean?
					Return field(name, New ArrowType.Bool())
				Case ColumnType.InnerEnum.Categorical
					Return field(name,New ArrowType.Utf8())
				Case ColumnType.InnerEnum.Time
					Return field(name,New ArrowType.Date(DateUnit.MILLISECOND))
				Case ColumnType.InnerEnum.Bytes
					Return field(name,New ArrowType.Binary())
				Case ColumnType.InnerEnum.NDArray
					Return field(name,New ArrowType.Binary())
				Case ColumnType.InnerEnum.String
					Return field(name,New ArrowType.Utf8())

				Case Else
					Throw New System.ArgumentException("Column type invalid " & columnType)
			End Select
		End Function

		''' <summary>
		''' Shortcut method for creating a double field
		''' with 64 bit floating point </summary>
		''' <param name="name"> the name of the field </param>
		''' <returns> the created field </returns>
		Public Shared Function doubleField(ByVal name As String) As Field
			Return getFieldForColumn(name, ColumnType.Double)
		End Function

		''' <summary>
		''' Shortcut method for creating a double field
		''' with 32 bit floating point </summary>
		''' <param name="name"> the name of the field </param>
		''' <returns> the created field </returns>
		Public Shared Function floatField(ByVal name As String) As Field
			Return getFieldForColumn(name,ColumnType.Float)
		End Function

		''' <summary>
		''' Shortcut method for creating a double field
		''' with 32 bit integer field </summary>
		''' <param name="name"> the name of the field </param>
		''' <returns> the created field </returns>
		Public Shared Function intField(ByVal name As String) As Field
			Return getFieldForColumn(name,ColumnType.Integer)
		End Function

		''' <summary>
		''' Shortcut method for creating a long field
		''' with 64 bit long field </summary>
		''' <param name="name"> the name of the field </param>
		''' <returns> the created field </returns>
		Public Shared Function longField(ByVal name As String) As Field
			Return getFieldForColumn(name,ColumnType.Long)
		End Function

		''' 
		''' <param name="name">
		''' @return </param>
		Public Shared Function stringField(ByVal name As String) As Field
			Return getFieldForColumn(name,ColumnType.String)
		End Function

		''' <summary>
		''' Shortcut </summary>
		''' <param name="name">
		''' @return </param>
		Public Shared Function booleanField(ByVal name As String) As Field
			Return getFieldForColumn(name,ColumnType.Boolean)
		End Function


		''' <summary>
		''' Provide a value look up dictionary based on the
		''' given set of input <seealso cref="FieldVector"/> s for
		''' reading and writing to arrow streams </summary>
		''' <param name="vectors"> the vectors to use as a lookup </param>
		''' <returns> the associated <seealso cref="DictionaryProvider"/> for the given
		''' input <seealso cref="FieldVector"/> list </returns>
		Public Shared Function providerForVectors(ByVal vectors As IList(Of FieldVector), ByVal fields As IList(Of Field)) As DictionaryProvider
			Dim dictionaries(vectors.Count - 1) As Dictionary
			For i As Integer = 0 To vectors.Count - 1
				Dim dictionary As DictionaryEncoding = fields(i).getDictionary()
				If dictionary Is Nothing Then
					dictionary = New DictionaryEncoding(i,True,Nothing)
				End If
				dictionaries(i) = New Dictionary(vectors(i), dictionary)
			Next i
			Return New DictionaryProvider.MapDictionaryProvider(dictionaries)
		End Function


		''' <summary>
		''' Given a buffer allocator and datavec schema,
		''' convert the passed in batch of records
		''' to a set of arrow columns </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="schema"> the schema to convert </param>
		''' <param name="dataVecRecord"> the data vec record batch to convert </param>
		''' <returns> the converted list of <seealso cref="FieldVector"/> </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static List<FieldVector> toArrowColumns(final org.apache.arrow.memory.BufferAllocator bufferAllocator, final org.datavec.api.transform.schema.Schema schema, List<List<Writable>> dataVecRecord)
		Public Shared Function toArrowColumns(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal dataVecRecord As IList(Of IList(Of Writable))) As IList(Of FieldVector)
			Dim numRows As Integer = dataVecRecord.Count

			Dim ret As IList(Of FieldVector) = createFieldVectors(bufferAllocator,schema,numRows)

			Dim j As Integer = 0
			Do While j < schema.numColumns()
				Dim fieldVector As FieldVector = ret(j)
				Dim row As Integer = 0
				For Each record As IList(Of Writable) In dataVecRecord
					Dim writable As Writable = record(j)
					setValue(schema.getType(j),fieldVector,writable,row)
					row += 1
				Next record

				j += 1
			Loop

			Return ret
		End Function


		''' <summary>
		''' Convert a set of input strings to arrow columns
		''' for a time series. </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <param name="dataVecRecord"> the collection of input strings to process </param>
		''' <returns> the created vectors </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static List<FieldVector> toArrowColumnsTimeSeries(final org.apache.arrow.memory.BufferAllocator bufferAllocator, final org.datavec.api.transform.schema.Schema schema, List<List<List<Writable>>> dataVecRecord)
		Public Shared Function toArrowColumnsTimeSeries(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal dataVecRecord As IList(Of IList(Of IList(Of Writable)))) As IList(Of FieldVector)
			Return toArrowColumnsTimeSeriesHelper(bufferAllocator,schema,dataVecRecord)
		End Function


		''' <summary>
		''' Convert a set of input strings to arrow columns
		''' for a time series. </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <param name="dataVecRecord"> the collection of input strings to process </param>
		''' <returns> the created vectors </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static <T> List<FieldVector> toArrowColumnsTimeSeriesHelper(final org.apache.arrow.memory.BufferAllocator bufferAllocator, final org.datavec.api.transform.schema.Schema schema, List<List<List<T>>> dataVecRecord)
		Public Shared Function toArrowColumnsTimeSeriesHelper(Of T)(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal dataVecRecord As IList(Of IList(Of IList(Of T)))) As IList(Of FieldVector)
			'time series length * number of columns
			Dim numRows As Integer = 0
			For Each timeStep As IList(Of IList(Of T)) In dataVecRecord
				numRows += timeStep(0).Count * timeStep.Count
			Next timeStep

			numRows \= schema.numColumns()


			Dim ret As IList(Of FieldVector) = createFieldVectors(bufferAllocator,schema,numRows)
			Dim currIndex As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)(ret.Count)
			For i As Integer = 0 To ret.Count - 1
				currIndex(i) = 0
			Next i
			For i As Integer = 0 To dataVecRecord.Count - 1
				Dim record As IList(Of IList(Of T)) = dataVecRecord(i)
				For j As Integer = 0 To record.Count - 1
					Dim curr As IList(Of T) = record(j)
					For k As Integer = 0 To curr.Count - 1
						Dim idx As Integer? = currIndex(k)
						Dim fieldVector As FieldVector = ret(k)
						Dim writable As T = curr(k)
						setValue(schema.getType(k), fieldVector, writable, idx)
						currIndex(k) = idx.Value + 1
					Next k
				Next j
			Next i

			Return ret
		End Function



		''' <summary>
		''' Convert a set of input strings to arrow columns </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <param name="dataVecRecord"> the collection of input strings to process </param>
		''' <returns> the created vectors </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static List<FieldVector> toArrowColumnsStringSingle(final org.apache.arrow.memory.BufferAllocator bufferAllocator, final org.datavec.api.transform.schema.Schema schema, List<String> dataVecRecord)
		Public Shared Function toArrowColumnsStringSingle(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal dataVecRecord As IList(Of String)) As IList(Of FieldVector)
			Return toArrowColumnsString(bufferAllocator, schema, New List(Of IList(Of String)) From {dataVecRecord})
		End Function



		''' <summary>
		''' Convert a set of input strings to arrow columns
		''' for a time series. </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <param name="dataVecRecord"> the collection of input strings to process </param>
		''' <returns> the created vectors </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static List<FieldVector> toArrowColumnsStringTimeSeries(final org.apache.arrow.memory.BufferAllocator bufferAllocator, final org.datavec.api.transform.schema.Schema schema, List<List<List<String>>> dataVecRecord)
		Public Shared Function toArrowColumnsStringTimeSeries(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal dataVecRecord As IList(Of IList(Of IList(Of String)))) As IList(Of FieldVector)
			Return toArrowColumnsTimeSeriesHelper(bufferAllocator,schema,dataVecRecord)

		End Function


		''' <summary>
		''' Convert a set of input strings to arrow columns </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="schema"> the schema to use </param>
		''' <param name="dataVecRecord"> the collection of input strings to process </param>
		''' <returns> the created vectors </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static List<FieldVector> toArrowColumnsString(final org.apache.arrow.memory.BufferAllocator bufferAllocator, final org.datavec.api.transform.schema.Schema schema, List<List<String>> dataVecRecord)
		Public Shared Function toArrowColumnsString(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal dataVecRecord As IList(Of IList(Of String))) As IList(Of FieldVector)
			Dim numRows As Integer = dataVecRecord.Count

			Dim ret As IList(Of FieldVector) = createFieldVectors(bufferAllocator,schema,numRows)
			''' <summary>
			''' Need to change iteration scheme
			''' </summary>

			Dim j As Integer = 0
			Do While j < schema.numColumns()
				Dim fieldVector As FieldVector = ret(j)
				For row As Integer = 0 To numRows - 1
					Dim writable As String = dataVecRecord(row)(j)
					setValue(schema.getType(j),fieldVector,writable,row)
				Next row

				j += 1
			Loop

			Return ret
		End Function


		Private Shared Function createFieldVectors(ByVal bufferAllocator As BufferAllocator, ByVal schema As Schema, ByVal numRows As Integer) As IList(Of FieldVector)
			Dim ret As IList(Of FieldVector) = New List(Of FieldVector)(schema.numColumns())

			Dim i As Integer = 0
			Do While i < schema.numColumns()
				Select Case schema.getType(i)
					Case Integer?
						ret.Add(intVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Long?
						ret.Add(longVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Double?
						ret.Add(doubleVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Single?
						ret.Add(floatVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Boolean?
						ret.Add(booleanVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case String
						ret.Add(stringVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Categorical
						ret.Add(stringVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Time
						ret.Add(timeVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case NDArray
						ret.Add(ndarrayVectorOf(bufferAllocator,schema.getName(i),numRows))
					Case Else
						Throw New System.ArgumentException("Illegal type found for creation of field vectors" & schema.getType(i))
				End Select
				i += 1
			Loop

			Return ret
		End Function

		''' <summary>
		''' Set the value of the specified column vector
		''' at the specified row based on the given value.
		''' The value will be converted relative to the specified column type.
		''' Note that the passed in value may only be a <seealso cref="Writable"/>
		''' or a <seealso cref="String"/> </summary>
		''' <param name="columnType"> the column type of the value </param>
		''' <param name="fieldVector"> the field vector to set </param>
		''' <param name="value"> the value to set (<seealso cref="Writable"/> or <seealso cref="String"/> types) </param>
		''' <param name="row"> the row of the item </param>
		Public Shared Sub setValue(ByVal columnType As ColumnType, ByVal fieldVector As FieldVector, ByVal value As Object, ByVal row As Integer)
			If TypeOf value Is NullWritable Then
				Return
			End If
			Try
				Select Case columnType.innerEnumValue
					Case Integer?
						If TypeOf fieldVector Is IntVector Then
							Dim intVector As IntVector = CType(fieldVector, IntVector)
							Dim set As Integer = TypeConversion.Instance.convertInt(value)
							intVector.set(row, set)
						ElseIf TypeOf fieldVector Is UInt4Vector Then
							Dim uInt4Vector As UInt4Vector = CType(fieldVector, UInt4Vector)
							Dim set As Integer = TypeConversion.Instance.convertInt(value)
							uInt4Vector.set(row, set)
						Else
							Throw New System.NotSupportedException("Illegal type " & fieldVector.GetType() & " for int type")
						End If
					Case Single?
						Dim float4Vector As Float4Vector = CType(fieldVector, Float4Vector)
						Dim set2 As Single = TypeConversion.Instance.convertFloat(value)
						float4Vector.set(row, set2)
					Case Double?
						Dim set3 As Double = TypeConversion.Instance.convertDouble(value)
						Dim float8Vector As Float8Vector = CType(fieldVector, Float8Vector)
						float8Vector.set(row, set3)
					Case Long?
						If TypeOf fieldVector Is BigIntVector Then
							Dim largeIntVector As BigIntVector = CType(fieldVector, BigIntVector)
							largeIntVector.set(row, TypeConversion.Instance.convertLong(value))

						ElseIf TypeOf fieldVector Is UInt8Vector Then
							Dim uInt8Vector As UInt8Vector = CType(fieldVector, UInt8Vector)
							uInt8Vector.set(row, TypeConversion.Instance.convertLong(value))
						Else
							Throw New System.NotSupportedException("Illegal type " & fieldVector.GetType() & " for long type")
						End If
					Case ColumnType.InnerEnum.Categorical, String
						Dim stringSet As String = TypeConversion.Instance.convertString(value)
						Dim textVector As VarCharVector = CType(fieldVector, VarCharVector)
						textVector.setSafe(row, stringSet.GetBytes())
					Case ColumnType.InnerEnum.Time
						'all timestamps are long based, just directly convert it to the super type
						Dim timeSet As Long = TypeConversion.Instance.convertLong(value)
						setLongInTime(fieldVector, row, timeSet)
					Case ColumnType.InnerEnum.NDArray
						Dim arr As NDArrayWritable = DirectCast(value, NDArrayWritable)
						Dim nd4jArrayVector As VarBinaryVector = CType(fieldVector, VarBinaryVector)
						'slice the databuffer to use only the needed portion of the buffer
						'for proper offsets
						Dim byteBuffer As ByteBuffer = BinarySerde.toByteBuffer(arr.get())
						nd4jArrayVector.setSafe(row,byteBuffer,0,byteBuffer.capacity())
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
					Case Boolean?
						Dim bitVector As BitVector = CType(fieldVector, BitVector)
						If TypeOf value Is Boolean? Then
							bitVector.set(row,If(DirectCast(value, Boolean), 1, 0))
						Else
							bitVector.set(row,If(DirectCast(value, BooleanWritable).get(), 1, 0))
						End If
				End Select
			Catch e As Exception
				log.warn("Unable to set value at row " & row)
			End Try
		End Sub


		Private Shared Sub setLongInTime(ByVal fieldVector As FieldVector, ByVal index As Integer, ByVal value As Long)
			If TypeOf fieldVector Is TimeStampMilliVector Then
				Dim timeStampMilliVector As TimeStampMilliVector = CType(fieldVector, TimeStampMilliVector)
				timeStampMilliVector.set(index,value)
			ElseIf TypeOf fieldVector Is TimeMilliVector Then
				Dim timeMilliVector As TimeMilliVector = CType(fieldVector, TimeMilliVector)
				timeMilliVector.set(index,CInt(value))
			ElseIf TypeOf fieldVector Is TimeStampMicroVector Then
				Dim timeStampMicroVector As TimeStampMicroVector = CType(fieldVector, TimeStampMicroVector)
				timeStampMicroVector.set(index,value)
			ElseIf TypeOf fieldVector Is TimeSecVector Then
				Dim timeSecVector As TimeSecVector = CType(fieldVector, TimeSecVector)
				timeSecVector.set(index,CInt(value))
			ElseIf TypeOf fieldVector Is TimeStampMilliVector Then
				Dim timeStampMilliVector As TimeStampMilliVector = CType(fieldVector, TimeStampMilliVector)
				timeStampMilliVector.set(index,value)
			ElseIf TypeOf fieldVector Is TimeStampMilliTZVector Then
				Dim timeStampMilliTZVector As TimeStampMilliTZVector = CType(fieldVector, TimeStampMilliTZVector)
				timeStampMilliTZVector.set(index, value)
			ElseIf TypeOf fieldVector Is TimeStampNanoTZVector Then
				Dim timeStampNanoTZVector As TimeStampNanoTZVector = CType(fieldVector, TimeStampNanoTZVector)
				timeStampNanoTZVector.set(index,value)
			ElseIf TypeOf fieldVector Is TimeStampMicroTZVector Then
				Dim timeStampMicroTZVector As TimeStampMicroTZVector = CType(fieldVector, TimeStampMicroTZVector)
				timeStampMicroTZVector.set(index,value)
			Else
				Throw New System.NotSupportedException()
			End If
		End Sub


		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As DateTime) As TimeStampMilliVector
			Dim float4Vector As New TimeStampMilliVector(name,allocator)
			float4Vector.allocateNew(data.Length)
			For i As Integer = 0 To data.Length - 1
				float4Vector.setSafe(i,data(i).Ticks)
			Next i

			float4Vector.setValueCount(data.Length)

			Return float4Vector
		End Function


		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="length"> the length of the vector
		''' @return </param>
		Public Shared Function timeVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As TimeStampMilliVector
			Dim float4Vector As New TimeStampMilliVector(name,allocator)
			float4Vector.allocateNew(length)
			float4Vector.setValueCount(length)
			Return float4Vector
		End Function


		''' <summary>
		''' Returns a vector representing a tensor view
		''' of each ndarray.
		''' Each ndarray will be a "row" represented as a tensor object
		''' with in the return <seealso cref="VarBinaryVector"/> </summary>
		''' <param name="bufferAllocator"> the buffer allocator to use </param>
		''' <param name="name"> the name of the column </param>
		''' <param name="data"> the input arrays
		''' @return </param>
		Public Shared Function vectorFor(ByVal bufferAllocator As BufferAllocator, ByVal name As String, ByVal data() As INDArray) As VarBinaryVector
			Dim ret As New VarBinaryVector(name,bufferAllocator)
			ret.allocateNew()
			For i As Integer = 0 To data.Length - 1
				'slice the databuffer to use only the needed portion of the buffer
				'for proper offset
				Dim byteBuffer As ByteBuffer = BinarySerde.toByteBuffer(data(i))
				ret.set(i,byteBuffer,0,byteBuffer.capacity())
			Next i

			Return ret
		End Function



		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As String) As VarCharVector
			Dim float4Vector As New VarCharVector(name,allocator)
			float4Vector.allocateNew()
			For i As Integer = 0 To data.Length - 1
				float4Vector.setSafe(i,data(i).GetBytes())
			Next i

			float4Vector.setValueCount(data.Length)

			Return float4Vector
		End Function


		''' <summary>
		''' Create an ndarray vector that stores structs
		''' of <seealso cref="INDArray"/>
		''' based on the <seealso cref="org.apache.arrow.flatbuf.Tensor"/>
		''' format </summary>
		''' <param name="allocator"> the allocator to use </param>
		''' <param name="name"> the name of the vector </param>
		''' <param name="length"> the number of vectors to store
		''' @return </param>
		Public Shared Function ndarrayVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As VarBinaryVector
			Dim ret As New VarBinaryVector(name,allocator)
			ret.allocateNewSafe()
			ret.setValueCount(length)
			Return ret
		End Function

		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="length"> the length of the vector
		''' @return </param>
		Public Shared Function stringVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As VarCharVector
			Dim float4Vector As New VarCharVector(name,allocator)
			float4Vector.allocateNew()
			float4Vector.setValueCount(length)
			Return float4Vector
		End Function



		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As Single) As Float4Vector
			Dim float4Vector As New Float4Vector(name,allocator)
			float4Vector.allocateNew(data.Length)
			For i As Integer = 0 To data.Length - 1
				float4Vector.setSafe(i,data(i))
			Next i

			float4Vector.setValueCount(data.Length)

			Return float4Vector
		End Function


		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="length"> the length of the vector
		''' @return </param>
		Public Shared Function floatVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As Float4Vector
			Dim float4Vector As New Float4Vector(name,allocator)
			float4Vector.allocateNew(length)
			float4Vector.setValueCount(length)
			Return float4Vector
		End Function

		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As Double) As Float8Vector
			Dim float8Vector As New Float8Vector(name,allocator)
			float8Vector.allocateNew(data.Length)
			For i As Integer = 0 To data.Length - 1
				float8Vector.setSafe(i,data(i))
			Next i


			float8Vector.setValueCount(data.Length)

			Return float8Vector
		End Function




		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="length"> the length of the vector
		''' @return </param>
		Public Shared Function doubleVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As Float8Vector
			Dim float8Vector As New Float8Vector(name,allocator)
			float8Vector.allocateNew()
			float8Vector.setValueCount(length)
			Return float8Vector
		End Function





		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As Boolean) As BitVector
			Dim float8Vector As New BitVector(name,allocator)
			float8Vector.allocateNew(data.Length)
			For i As Integer = 0 To data.Length - 1
				float8Vector.setSafe(i,If(data(i), 1, 0))
			Next i

			float8Vector.setValueCount(data.Length)

			Return float8Vector
		End Function

		''' 
		''' <param name="allocator"> </param>
		''' <param name="name">
		''' @return </param>
		Public Shared Function booleanVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As BitVector
			Dim float8Vector As New BitVector(name,allocator)
			float8Vector.allocateNew(length)
			float8Vector.setValueCount(length)
			Return float8Vector
		End Function


		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As Integer) As IntVector
			Dim float8Vector As New IntVector(name,FieldType.nullable(New ArrowType.Int(32,True)),allocator)
			float8Vector.allocateNew(data.Length)
			For i As Integer = 0 To data.Length - 1
				float8Vector.setSafe(i,data(i))
			Next i

			float8Vector.setValueCount(data.Length)

			Return float8Vector
		End Function

		''' 
		''' <param name="allocator"> </param>
		''' <param name="name">
		''' @return </param>
		Public Shared Function intVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As IntVector
			Dim float8Vector As New IntVector(name,FieldType.nullable(New ArrowType.Int(32,True)),allocator)
			float8Vector.allocateNew(length)

			float8Vector.setValueCount(length)

			Return float8Vector
		End Function




		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="data">
		''' @return </param>
		Public Shared Function vectorFor(ByVal allocator As BufferAllocator, ByVal name As String, ByVal data() As Long) As BigIntVector
			Dim float8Vector As New BigIntVector(name,FieldType.nullable(New ArrowType.Int(64,True)),allocator)
			float8Vector.allocateNew(data.Length)
			For i As Integer = 0 To data.Length - 1
				float8Vector.setSafe(i,data(i))
			Next i

			float8Vector.setValueCount(data.Length)

			Return float8Vector
		End Function



		''' 
		''' <param name="allocator"> </param>
		''' <param name="name"> </param>
		''' <param name="length"> the number of rows in the column vector
		''' @return </param>
		Public Shared Function longVectorOf(ByVal allocator As BufferAllocator, ByVal name As String, ByVal length As Integer) As BigIntVector
			Dim float8Vector As New BigIntVector(name,FieldType.nullable(New ArrowType.Int(64,True)),allocator)
			float8Vector.allocateNew(length)
			float8Vector.setValueCount(length)
			Return float8Vector
		End Function

		Private Shared Function metaDataFromField(ByVal field As Field) As ColumnMetaData
			Dim arrowType As ArrowType = field.getFieldType().getType()
			If TypeOf arrowType Is ArrowType.Int Then
				Dim intType As val = CType(arrowType, ArrowType.Int)
				If intType.getBitWidth() = 32 Then
					Return New IntegerMetaData(field.getName())
				Else
					Return New LongMetaData(field.getName())
				End If
			ElseIf TypeOf arrowType Is ArrowType.Bool Then
				Return New BooleanMetaData(field.getName())
			ElseIf TypeOf arrowType Is ArrowType.FloatingPoint Then
				Dim floatingPointType As val = CType(arrowType, ArrowType.FloatingPoint)
				If floatingPointType.getPrecision() = FloatingPointPrecision.DOUBLE Then
					Return New DoubleMetaData(field.getName())
				Else
					Return New FloatMetaData(field.getName())
				End If
			ElseIf TypeOf arrowType Is ArrowType.Binary Then
				Return New BinaryMetaData(field.getName())
			ElseIf TypeOf arrowType Is ArrowType.Utf8 Then
				Return New StringMetaData(field.getName())

			ElseIf TypeOf arrowType Is ArrowType.Date Then
				Return New TimeMetaData(field.getName())
			Else
				Throw New System.InvalidOperationException("Illegal type " & field.getFieldType().getType())
			End If

		End Function


		''' <summary>
		''' Based on an input <seealso cref="ColumnType"/>
		''' get an entry from a <seealso cref="FieldVector"/>
		''' </summary>
		''' <param name="item"> the row of the item to get from the column vector </param>
		''' <param name="from"> the column vector from </param>
		''' <param name="columnType"> the column type </param>
		''' <returns> the resulting writable </returns>
		Public Shared Function fromEntry(ByVal item As Integer, ByVal from As FieldVector, ByVal columnType As ColumnType) As Writable
			If from.getValueCount() < item Then
				Throw New System.ArgumentException("Index specified greater than the number of items in the vector with length " & from.getValueCount())
			End If

			Select Case columnType.innerEnumValue
				Case Integer?
					Return New IntWritable(getIntFromFieldVector(item,from))
				Case Long?
					Return New LongWritable(getLongFromFieldVector(item,from))
				Case Single?
					Return New FloatWritable(getFloatFromFieldVector(item,from))
				Case Double?
					Return New DoubleWritable(getDoubleFromFieldVector(item,from))
				Case Boolean?
					Dim bitVector As BitVector = CType(from, BitVector)
					Return New BooleanWritable(bitVector.get(item) > 0)
				Case ColumnType.InnerEnum.Categorical
					Dim varCharVector As VarCharVector = CType(from, VarCharVector)
					Return New Text(varCharVector.get(item))
				Case ColumnType.InnerEnum.String
					Dim varCharVector2 As VarCharVector = CType(from, VarCharVector)
					Return New Text(varCharVector2.get(item))
				Case ColumnType.InnerEnum.Time
					'TODO: need to look at closer
					Return New LongWritable(getLongFromFieldVector(item,from))
				Case ColumnType.InnerEnum.NDArray
					Dim valueVector As VarBinaryVector = CType(from, VarBinaryVector)
					Dim bytes() As SByte = valueVector.get(item)
					Dim direct As ByteBuffer = ByteBuffer.allocateDirect(bytes.Length)
					direct.put(bytes)
					Dim fromTensor As INDArray = BinarySerde.toArray(direct)
					Return New NDArrayWritable(fromTensor)
				Case Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New System.ArgumentException("Illegal type " & from.GetType().FullName)
			End Select
		End Function


		Private Shared Function getIntFromFieldVector(ByVal row As Integer, ByVal fieldVector As FieldVector) As Integer
			If TypeOf fieldVector Is UInt4Vector Then
				Dim uInt4Vector As UInt4Vector = CType(fieldVector, UInt4Vector)
				Return uInt4Vector.get(row)
			ElseIf TypeOf fieldVector Is IntVector Then
				Dim intVector As IntVector = CType(fieldVector, IntVector)
				Return intVector.get(row)
			End If

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.ArgumentException("Illegal vector type for int " & fieldVector.GetType().FullName)
		End Function

		Private Shared Function getLongFromFieldVector(ByVal row As Integer, ByVal fieldVector As FieldVector) As Long
			If TypeOf fieldVector Is UInt8Vector Then
				Dim uInt4Vector As UInt8Vector = CType(fieldVector, UInt8Vector)
				Return uInt4Vector.get(row)
			ElseIf TypeOf fieldVector Is IntVector Then
				Dim intVector As BigIntVector = CType(fieldVector, BigIntVector)
				Return intVector.get(row)
			ElseIf TypeOf fieldVector Is TimeStampMilliVector Then
				Dim timeStampMilliVector As TimeStampMilliVector = CType(fieldVector, TimeStampMilliVector)
				Return timeStampMilliVector.get(row)
			ElseIf TypeOf fieldVector Is BigIntVector Then
				Dim bigIntVector As BigIntVector = CType(fieldVector, BigIntVector)
				Return bigIntVector.get(row)
			ElseIf TypeOf fieldVector Is DateMilliVector Then
				Dim dateMilliVector As DateMilliVector = CType(fieldVector, DateMilliVector)
				Return dateMilliVector.get(row)

			ElseIf TypeOf fieldVector Is TimeMilliVector Then
				Dim timeMilliVector As TimeMilliVector = CType(fieldVector, TimeMilliVector)
				Return timeMilliVector.get(row)
			ElseIf TypeOf fieldVector Is TimeStampMicroVector Then
				Dim timeStampMicroVector As TimeStampMicroVector = CType(fieldVector, TimeStampMicroVector)
				Return timeStampMicroVector.get(row)
			ElseIf TypeOf fieldVector Is TimeSecVector Then
				Dim timeSecVector As TimeSecVector = CType(fieldVector, TimeSecVector)
				Return timeSecVector.get(row)
			ElseIf TypeOf fieldVector Is TimeStampMilliTZVector Then
				Dim timeStampMilliTZVector As TimeStampMilliTZVector = CType(fieldVector, TimeStampMilliTZVector)
				Return timeStampMilliTZVector.get(row)
			ElseIf TypeOf fieldVector Is TimeStampNanoTZVector Then
				Dim timeStampNanoTZVector As TimeStampNanoTZVector = CType(fieldVector, TimeStampNanoTZVector)
				Return timeStampNanoTZVector.get(row)
			ElseIf TypeOf fieldVector Is TimeStampMicroTZVector Then
				Dim timeStampMicroTZVector As TimeStampMicroTZVector = CType(fieldVector, TimeStampMicroTZVector)
				Return timeStampMicroTZVector.get(row)
			Else
				Throw New System.NotSupportedException()
			End If

		End Function

		Private Shared Function getDoubleFromFieldVector(ByVal row As Integer, ByVal fieldVector As FieldVector) As Double
			If TypeOf fieldVector Is Float8Vector Then
				Dim uInt4Vector As Float8Vector = CType(fieldVector, Float8Vector)
				Return uInt4Vector.get(row)
			End If


'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.ArgumentException("Illegal vector type for int " & fieldVector.GetType().FullName)
		End Function


		Private Shared Function getFloatFromFieldVector(ByVal row As Integer, ByVal fieldVector As FieldVector) As Single
			If TypeOf fieldVector Is Float4Vector Then
				Dim uInt4Vector As Float4Vector = CType(fieldVector, Float4Vector)
				Return uInt4Vector.get(row)
			End If


'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.ArgumentException("Illegal vector type for int " & fieldVector.GetType().FullName)
		End Function


		Private Shared Function asDataVecBatch(ByVal arrowRecordBatch As ArrowRecordBatch, ByVal schema As Schema, ByVal vectorLoader As VectorSchemaRoot) As ArrowWritableRecordBatch
			'iterate column wise over the feature vectors, returning entries
			Dim fieldVectors As IList(Of FieldVector) = New List(Of FieldVector)()
			Dim j As Integer = 0
			Do While j < schema.numColumns()
				Dim name As String = schema.getName(j)
				Dim fieldVector As FieldVector = vectorLoader.getVector(name)
				fieldVectors.Add(fieldVector)
				j += 1
			Loop

			Dim ret As New ArrowWritableRecordBatch(fieldVectors, schema)
			ret.setArrowRecordBatch(arrowRecordBatch)

			Return ret
		End Function



	End Class

End Namespace