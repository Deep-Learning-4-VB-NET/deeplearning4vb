Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Preconditions = org.nd4j.shade.guava.base.Preconditions
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Configuration = org.datavec.api.conf.Configuration
Imports PathLabelGenerator = org.datavec.api.io.labels.PathLabelGenerator
Imports PathMultiLabelGenerator = org.datavec.api.io.labels.PathMultiLabelGenerator
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = org.datavec.api.records.metadata.RecordMetaDataURI
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports InputSplit = org.datavec.api.split.InputSplit
Imports InputStreamInputSplit = org.datavec.api.split.InputStreamInputSplit
Imports FileFromPathIterator = org.datavec.api.util.files.FileFromPathIterator
Imports URIUtil = org.datavec.api.util.files.URIUtil
Imports RecordConverter = org.datavec.api.util.ndarray.RecordConverter
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports NDArrayRecordBatch = org.datavec.api.writable.batch.NDArrayRecordBatch
Imports BaseImageLoader = org.datavec.image.loader.BaseImageLoader
Imports ImageLoader = org.datavec.image.loader.ImageLoader
Imports NativeImageLoader = org.datavec.image.loader.NativeImageLoader
Imports ImageTransform = org.datavec.image.transform.ImageTransform
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.datavec.image.recordreader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseImageRecordReader extends org.datavec.api.records.reader.BaseRecordReader
	<Serializable>
	Public MustInherit Class BaseImageRecordReader
		Inherits BaseRecordReader

		Protected Friend finishedInputStreamSplit As Boolean
		Protected Friend iter As IEnumerator(Of File)
'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend conf_Conflict As Configuration
'JAVA TO VB CONVERTER NOTE: The field currentFile was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend currentFile_Conflict As File
		Protected Friend labelGenerator As PathLabelGenerator = Nothing
		Protected Friend labelMultiGenerator As PathMultiLabelGenerator = Nothing
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend labels_Conflict As IList(Of String) = New List(Of String)()
		Protected Friend appendLabel As Boolean = False
		Protected Friend writeLabel As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field record was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend record_Conflict As IList(Of Writable)
		Protected Friend hitImage As Boolean = False
		Protected Friend height As Long = 28, width As Long = 28, channels As Long = 1
		Protected Friend cropImage As Boolean = False
		Protected Friend imageTransform As ImageTransform
		Protected Friend imageLoader As BaseImageLoader
		Protected Friend Shadows inputSplit As org.datavec.api.Split.InputSplit
		Protected Friend fileNameMap As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
		Protected Friend pattern As String ' Pattern to split and segment file name, pass in regex
		Protected Friend patternPosition As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean logLabelCountOnInit = true;
		Protected Friend logLabelCountOnInit As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected boolean nchw_channels_first = true;
		Protected Friend nchw_channels_first As Boolean = True

'JAVA TO VB CONVERTER NOTE: The field HEIGHT was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly HEIGHT_Conflict As String = NAME_SPACE & ".height"
'JAVA TO VB CONVERTER NOTE: The field WIDTH was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly WIDTH_Conflict As String = NAME_SPACE & ".width"
'JAVA TO VB CONVERTER NOTE: The field CHANNELS was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Public Shared ReadOnly CHANNELS_Conflict As String = NAME_SPACE & ".channels"
		Public Shared ReadOnly CROP_IMAGE As String = NAME_SPACE & ".cropimage"
		Public Shared ReadOnly IMAGE_LOADER As String = NAME_SPACE & ".imageloader"

		Public Sub New()
		End Sub

		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathLabelGenerator)
			Me.New(height, width, channels, labelGenerator, Nothing)
		End Sub

		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathMultiLabelGenerator)
			Me.New(height, width, channels, Nothing, labelGenerator,Nothing)
		End Sub

		Public Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathLabelGenerator, ByVal imageTransform As ImageTransform)
			Me.New(height, width, channels, labelGenerator, Nothing, imageTransform)
		End Sub

		Protected Friend Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal labelGenerator As PathLabelGenerator, ByVal labelMultiGenerator As PathMultiLabelGenerator, ByVal imageTransform As ImageTransform)
			Me.New(height, width, channels, True, labelGenerator, labelMultiGenerator, imageTransform)
		End Sub

		Protected Friend Sub New(ByVal height As Long, ByVal width As Long, ByVal channels As Long, ByVal nchw_channels_first As Boolean, ByVal labelGenerator As PathLabelGenerator, ByVal labelMultiGenerator As PathMultiLabelGenerator, ByVal imageTransform As ImageTransform)
			Me.height = height
			Me.width = width
			Me.channels = channels
			Me.labelGenerator = labelGenerator
			Me.labelMultiGenerator = labelMultiGenerator
			Me.imageTransform = imageTransform
			Me.appendLabel = (labelGenerator IsNot Nothing OrElse labelMultiGenerator IsNot Nothing)
			Me.nchw_channels_first = nchw_channels_first
		End Sub

		Protected Friend Overridable Function containsFormat(ByVal format As String) As Boolean
			For Each format2 As String In imageLoader.AllowedFormats
				If format.EndsWith("." & format2, StringComparison.Ordinal) Then
					Return True
				End If
			Next format2
			Return False
		End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			If imageLoader Is Nothing Then
				imageLoader = New NativeImageLoader(height, width, channels, imageTransform)
			End If

			If TypeOf split Is org.datavec.api.Split.InputStreamInputSplit Then
				Me.inputSplit = split
				Me.finishedInputStreamSplit = False
				Return
			End If

			inputSplit = split



			Dim locations() As URI = split.locations()
			If locations IsNot Nothing AndAlso locations.Length >= 1 Then
				If appendLabel AndAlso labelGenerator IsNot Nothing AndAlso labelGenerator.inferLabelClasses() Then
					Dim labelsSet As ISet(Of String) = New HashSet(Of String)()
					For Each location As URI In locations
						Dim imgFile As New File(location)
						Dim name As String = labelGenerator.getLabelForPath(location).ToString()
						labelsSet.Add(name)
						If pattern IsNot Nothing Then
							Dim label As String = name.Split(pattern, True)(patternPosition)
							fileNameMap(imgFile.ToString()) = label
						End If
					Next location
					labels_Conflict.Clear()
					CType(labels_Conflict, List(Of String)).AddRange(labelsSet)
					If logLabelCountOnInit Then
						log.info("ImageRecordReader: {} label classes inferred using label generator {}", labelsSet.Count, labelGenerator.GetType().Name)
					End If
				End If
				iter = New FileFromPathIterator(inputSplit.locationsPathIterator()) 'This handles randomization internally if necessary
			Else
				Throw New System.ArgumentException("No path locations found in the split.")
			End If

			If TypeOf split Is org.datavec.api.Split.FileSplit Then
				'remove the root directory
				Dim split1 As org.datavec.api.Split.FileSplit = DirectCast(split, org.datavec.api.Split.FileSplit)
				labels_Conflict.Remove(split1.RootDir)
			End If

			'To ensure consistent order for label assignment (irrespective of file iteration order), we want to sort the list of labels
			labels_Conflict.Sort()
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			Me.appendLabel = conf.getBoolean(APPEND_LABEL, appendLabel)
			Me.labels_Conflict = New List(Of String)(conf.getStringCollection(LABELS))
			Me.height = conf.getLong(HEIGHT_Conflict, height)
			Me.width = conf.getLong(WIDTH_Conflict, width)
			Me.channels = conf.getLong(CHANNELS_Conflict, channels)
			Me.cropImage = conf.getBoolean(CROP_IMAGE, cropImage)
			If "imageio".Equals(conf.get(IMAGE_LOADER)) Then
				Me.imageLoader = New ImageLoader(height, width, channels, cropImage)
			Else
				Me.imageLoader = New NativeImageLoader(height, width, channels, imageTransform)
			End If
			Me.conf_Conflict = conf
			initialize(split)
		End Sub


		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="split">          the split that defines the range of records to read </param>
		''' <param name="imageTransform"> the image transform to use to transform images while loading them </param>
		''' <exception cref="java.io.IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void initialize(org.datavec.api.split.InputSplit split, org.datavec.image.transform.ImageTransform imageTransform) throws IOException
		Public Overridable Overloads Sub initialize(ByVal split As InputSplit, ByVal imageTransform As ImageTransform)
			Me.imageLoader = Nothing
			Me.imageTransform = imageTransform
			initialize(split)
		End Sub

		''' <summary>
		''' Called once at initialization.
		''' </summary>
		''' <param name="conf">           a configuration for initialization </param>
		''' <param name="split">          the split that defines the range of records to read </param>
		''' <param name="imageTransform"> the image transform to use to transform images while loading them </param>
		''' <exception cref="java.io.IOException"> </exception>
		''' <exception cref="InterruptedException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split, org.datavec.image.transform.ImageTransform imageTransform) throws IOException, InterruptedException
		Public Overridable Overloads Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit, ByVal imageTransform As ImageTransform)
			Me.imageLoader = Nothing
			Me.imageTransform = imageTransform
			initialize(conf, split)
		End Sub


		Public Overrides Function [next]() As IList(Of Writable)
			If TypeOf inputSplit Is org.datavec.api.Split.InputStreamInputSplit Then
				Dim inputStreamInputSplit As org.datavec.api.Split.InputStreamInputSplit = DirectCast(inputSplit, org.datavec.api.Split.InputStreamInputSplit)
				Try
					Dim ndArrayWritable As New NDArrayWritable(imageLoader.asMatrix(inputStreamInputSplit.Is))
					finishedInputStreamSplit = True
					Return New List(Of Writable) From {Of Writable}
				Catch e As IOException
					log.error("",e)
				End Try
			End If
			If iter IsNot Nothing Then
				Dim ret As IList(Of Writable)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim image As File = iter.next()
				currentFile_Conflict = image

				If image.isDirectory() Then
					Return [next]()
				End If
				Try
					invokeListeners(image)
					Dim array As INDArray = imageLoader.asMatrix(image)
					If Not nchw_channels_first Then
						array = array.permute(0,2,3,1) 'NCHW to NHWC
					End If

					Nd4j.AffinityManager.ensureLocation(array, AffinityManager.Location.DEVICE)
					ret = RecordConverter.toRecord(array)
					If appendLabel OrElse writeLabel Then
						If labelMultiGenerator IsNot Nothing Then
							CType(ret, List(Of Writable)).AddRange(labelMultiGenerator.getLabels(image.getPath()))
						Else
							If labelGenerator.inferLabelClasses() Then
								'Standard classification use case (i.e., handle String -> integer conversion
								ret.Add(New IntWritable(labels_Conflict.IndexOf(getLabel(image.getPath()))))
							Else
								'Regression use cases, and PathLabelGenerator instances that already map to integers
								ret.Add(labelGenerator.getLabelForPath(image.getPath()))
							End If
						End If
					End If
				Catch e As Exception
					Throw New Exception(e)
				End Try
				Return ret
			ElseIf record_Conflict IsNot Nothing Then
				hitImage = True
				invokeListeners(record_Conflict)
				Return record_Conflict
			End If
			Throw New System.InvalidOperationException("No more elements")
		End Function

		Public Overrides Function hasNext() As Boolean
			If TypeOf inputSplit Is org.datavec.api.Split.InputStreamInputSplit Then
				Return finishedInputStreamSplit
			End If

			If iter IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Return iter.hasNext()
			ElseIf record_Conflict IsNot Nothing Then
				Return Not hitImage
			End If
			Throw New System.InvalidOperationException("Indeterminant state: record must not be null, or a file iterator must exist")
		End Function

		Public Overrides Function batchesSupported() As Boolean
			Return (TypeOf imageLoader Is NativeImageLoader)
		End Function

		Public Overrides Function [next](ByVal num As Integer) As IList(Of IList(Of Writable))
			Preconditions.checkArgument(num > 0, "Number of examples must be > 0: got %s", num)

			If imageLoader Is Nothing Then
				imageLoader = New NativeImageLoader(height, width, channels, imageTransform)
			End If

			Dim currBatch As IList(Of File) = New List(Of File)()

			Dim cnt As Integer = 0

			Dim numCategories As Integer = If(appendLabel OrElse writeLabel, labels_Conflict.Count, 0)
			Dim currLabels As IList(Of Integer) = Nothing
			Dim currLabelsWritable As IList(Of Writable) = Nothing
			Dim multiGenLabels As IList(Of IList(Of Writable)) = Nothing
			Do While cnt < num AndAlso iter.MoveNext()
				currentFile_Conflict = iter.Current
				currBatch.Add(currentFile_Conflict)
				invokeListeners(currentFile_Conflict)
				If appendLabel OrElse writeLabel Then
					'Collect the label Writables from the label generators
					If labelMultiGenerator IsNot Nothing Then
						If multiGenLabels Is Nothing Then
							multiGenLabels = New List(Of IList(Of Writable))()
						End If

						multiGenLabels.Add(labelMultiGenerator.getLabels(currentFile_Conflict.getPath()))
					Else
						If labelGenerator.inferLabelClasses() Then
							If currLabels Is Nothing Then
								currLabels = New List(Of Integer)()
							End If
							currLabels.Add(labels_Conflict.IndexOf(getLabel(currentFile_Conflict.getPath())))
						Else
							If currLabelsWritable Is Nothing Then
								currLabelsWritable = New List(Of Writable)()
							End If
							currLabelsWritable.Add(labelGenerator.getLabelForPath(currentFile_Conflict.getPath()))
						End If
					End If
				End If
				cnt += 1
			Loop

			Dim features As INDArray = Nd4j.createUninitialized(New Long() {cnt, channels, height, width}, "c"c)
			Nd4j.AffinityManager.tagLocation(features, AffinityManager.Location.HOST)
			For i As Integer = 0 To cnt - 1
				Try
					DirectCast(imageLoader, NativeImageLoader).asMatrixView(currBatch(i), features.tensorAlongDimension(i, 1, 2, 3))
				Catch e As Exception
					Console.WriteLine("Image file failed during load: " & currBatch(i).getAbsolutePath())
					Throw New Exception(e)
				End Try
			Next i
			If Not nchw_channels_first Then
				features = features.permute(0,2,3,1) 'NCHW to NHWC
			End If
			Nd4j.AffinityManager.ensureLocation(features, AffinityManager.Location.DEVICE)


			Dim ret As IList(Of INDArray) = New List(Of INDArray)()
			ret.Add(features)
			If appendLabel OrElse writeLabel Then
				'And convert the previously collected label Writables from the label generators
				If labelMultiGenerator IsNot Nothing Then
					Dim temp As IList(Of Writable) = New List(Of Writable)()
					Dim first As IList(Of Writable) = multiGenLabels(0)
					For col As Integer = 0 To first.Count - 1
						temp.Clear()
						For Each multiGenLabel As IList(Of Writable) In multiGenLabels
							temp.Add(multiGenLabel(col))
						Next multiGenLabel
						Dim currCol As INDArray = RecordConverter.toMinibatchArray(temp)
						ret.Add(currCol)
					Next col
				Else
					Dim labels As INDArray
					If labelGenerator.inferLabelClasses() Then
						'Standard classification use case (i.e., handle String -> integer conversion)
						labels = Nd4j.create(cnt, numCategories, "c"c)
						Nd4j.AffinityManager.tagLocation(labels, AffinityManager.Location.HOST)
						For i As Integer = 0 To currLabels.Count - 1
							labels.putScalar(i, currLabels(i), 1.0f)
						Next i
					Else
						'Regression use cases, and PathLabelGenerator instances that already map to integers
						If TypeOf currLabelsWritable(0) Is NDArrayWritable Then
							Dim arr As IList(Of INDArray) = New List(Of INDArray)()
							For Each w As Writable In currLabelsWritable
								arr.Add(DirectCast(w, NDArrayWritable).get())
							Next w
							labels = Nd4j.concat(0, CType(arr, List(Of INDArray)).ToArray())
						Else
							labels = RecordConverter.toMinibatchArray(currLabelsWritable)
						End If
					End If

					ret.Add(labels)
				End If
			End If

			Return New NDArrayRecordBatch(ret)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws IOException
		Public Overridable Sub Dispose()
			'No op
		End Sub

		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.conf_Conflict = conf
			End Set
			Get
				Return conf_Conflict
			End Get
		End Property



		''' <summary>
		''' Get the label from the given path
		''' </summary>
		''' <param name="path"> the path to get the label from </param>
		''' <returns> the label for the given path </returns>
		Public Overridable Function getLabel(ByVal path As String) As String
			If labelGenerator IsNot Nothing Then
				Return labelGenerator.getLabelForPath(path).ToString()
			End If
			If fileNameMap IsNot Nothing AndAlso fileNameMap.ContainsKey(path) Then
				Return fileNameMap(path)
			End If
			Return (New File(path)).getParentFile().getName()
		End Function

		''' <summary>
		''' Accumulate the label from the path
		''' </summary>
		''' <param name="path"> the path to get the label from </param>
		Protected Friend Overridable Sub accumulateLabel(ByVal path As String)
			Dim name As String = getLabel(path)
			If Not labels_Conflict.Contains(name) Then
				labels_Conflict.Add(name)
			End If
		End Sub

		''' <summary>
		''' Returns the file loaded last by <seealso cref="next()"/>.
		''' </summary>
		Public Overridable Property CurrentFile As File
			Get
				Return currentFile_Conflict
			End Get
			Set(ByVal currentFile As File)
				Me.currentFile_Conflict = currentFile
			End Set
		End Property


		Public Overrides Property Labels As IList(Of String)
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels As IList(Of String))
				Me.labels_Conflict = labels
				Me.writeLabel = True
			End Set
		End Property


		Public Overrides Sub reset()
			If inputSplit Is Nothing Then
				Throw New System.NotSupportedException("Cannot reset without first initializing")
			End If
			inputSplit.reset()
			If iter IsNot Nothing Then
				iter = New FileFromPathIterator(inputSplit.locationsPathIterator())
			ElseIf record_Conflict IsNot Nothing Then
				hitImage = False
			End If
		End Sub

		Public Overrides Function resetSupported() As Boolean
			If inputSplit Is Nothing Then
				Return False
			End If
			Return inputSplit.resetSupported()
		End Function

		''' <summary>
		''' Returns {@code getLabels().size()}.
		''' </summary>
		Public Overridable Function numLabels() As Integer
			Return labels_Conflict.Count
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.writable.Writable> record(java.net.URI uri, DataInputStream dataInputStream) throws IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			invokeListeners(uri)
			If imageLoader Is Nothing Then
				imageLoader = New NativeImageLoader(height, width, channels, imageTransform)
			End If
			Dim array As INDArray = imageLoader.asMatrix(dataInputStream)
			If Not nchw_channels_first Then
				array = array.permute(0,2,3,1)
			End If
			Dim ret As IList(Of Writable) = RecordConverter.toRecord(array)
			If appendLabel Then
				ret.Add(New IntWritable(labels_Conflict.IndexOf(getLabel(uri.getPath()))))
			End If
			Return ret
		End Function

		Public Overrides Function nextRecord() As Record
			Dim list As IList(Of Writable) = [next]()
			Dim uri As URI = URIUtil.fileToURI(currentFile_Conflict)
			Return New org.datavec.api.records.impl.Record(list, New RecordMetaDataURI(uri, GetType(BaseImageRecordReader)))
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Return loadFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public List<org.datavec.api.records.Record> loadFromMetaData(List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Dim [out] As IList(Of Record) = New List(Of Record)()
			For Each meta As RecordMetaData In recordMetaDatas
				Dim uri As URI = meta.URI
				Dim f As New File(uri)

				Dim [next] As IList(Of Writable)
				Using dis As New DataInputStream(New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read)))
					[next] = record(uri, dis)
				End Using
				[out].Add(New org.datavec.api.records.impl.Record([next], meta))
			Next meta
			Return [out]
		End Function
	End Class

End Namespace