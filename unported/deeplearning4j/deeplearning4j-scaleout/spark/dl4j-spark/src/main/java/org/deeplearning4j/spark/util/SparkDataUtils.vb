Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports RecordReaderFileBatchLoader = org.deeplearning4j.core.loader.impl.RecordReaderFileBatchLoader
Imports FileBatch = org.nd4j.common.loader.FileBatch

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

Namespace org.deeplearning4j.spark.util


	Public Class SparkDataUtils

		Private Sub New()
		End Sub

		''' <summary>
		''' See <seealso cref="createFileBatchesLocal(File, String[], Boolean, File, Integer)"/>.<br>
		''' The directory filtering (extensions arg) is null when calling this method.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void createFileBatchesLocal(File inputDirectory, boolean recursive, File outputDirectory, int batchSize) throws IOException
		Public Shared Sub createFileBatchesLocal(ByVal inputDirectory As File, ByVal recursive As Boolean, ByVal outputDirectory As File, ByVal batchSize As Integer)
			createFileBatchesLocal(inputDirectory, Nothing, recursive, outputDirectory, batchSize)
		End Sub

		''' <summary>
		''' Create a number of <seealso cref="FileBatch"/> files from local files (in random order).<br>
		''' Use cases: distributed training on compressed file formats such as images, that need to be loaded to a remote
		''' file storage system such as HDFS. Local files can be created using this method and then copied to HDFS for training.<br>
		''' FileBatch is also compressed (zip file format) so space may be saved in some cases (such as CSV sequences)
		''' For example, if we were training with a minibatch size of 64 images, reading the raw images would result in 64
		''' different disk reads (one for each file) - which could clearly be a bottleneck during training.<br>
		''' Alternatively, we could create and save DataSet/INDArray objects containing a batch of images - however, storing
		''' images in FP32 (or ever UINT8) format - effectively a bitmap - is still much less efficient than the raw image files.<br>
		''' Instead, can create minibatches of <seealso cref="FileBatch"/> objects: these objects contain the raw file content for
		''' multiple files (as byte[]s) along with their original paths, which can then be used for distributed training using
		''' <seealso cref="RecordReaderFileBatchLoader"/>.<br>
		''' This approach gives us the benefits of the original file format (i.e., small size, compression) along with
		''' the benefits of a batched DataSet/INDArray format - i.e., disk reads are reduced by a factor of the minibatch size.<br>
		''' <br>
		''' See <seealso cref="createFileBatchesSpark(JavaRDD, String, Integer, JavaSparkContext)"/> for the distributed (Spark) version of this method.<br>
		''' <br>
		''' Usage - image classification example - assume each FileBatch object contains a number of jpg/png etc image files
		''' <pre>
		''' {@code
		''' JavaSparkContext sc = ...
		''' SparkDl4jMultiLayer net = ...
		''' String baseFileBatchDir = ...
		''' JavaRDD<String> paths = org.deeplearning4j.spark.util.SparkUtils.listPaths(sc, baseFileBatchDir);
		''' 
		''' //Image record reader:
		''' PathLabelGenerator labelMaker = new ParentPathLabelGenerator();
		''' ImageRecordReader rr = new ImageRecordReader(32, 32, 1, labelMaker);
		''' rr.setLabels(<labels here>);
		''' 
		''' //Create DataSetLoader:
		''' int batchSize = 32;
		''' int numClasses = 1000;
		''' DataSetLoader loader = RecordReaderFileBatchLoader(rr, batchSize, 1, numClasses);
		''' 
		''' //Fit the network
		''' net.fitPaths(paths, loader);
		''' }
		''' </pre>
		''' </summary>
		''' <param name="inputDirectory">  Directory containing the files to convert </param>
		''' <param name="extensions">      Optional (may be null). If non-null, only those files with the specified extension will be included </param>
		''' <param name="recursive">       If true: convert the files recursively </param>
		''' <param name="outputDirectory"> Output directory to save the created FileBatch objects </param>
		''' <param name="batchSize">       Batch size - i.e., minibatch size to be used for training, and the number of files to
		'''                        include in each FileBatch object </param>
		''' <exception cref="IOException"> If an error occurs while reading the files </exception>
		''' <seealso cref= #createFileBatchesSpark(JavaRDD, String, int, JavaSparkContext) </seealso>
		''' <seealso cref= org.datavec.api.records.reader.impl.filebatch.FileBatchRecordReader FileBatchRecordReader for local training on these files, if required </seealso>
		''' <seealso cref= org.datavec.api.records.reader.impl.filebatch.FileBatchSequenceRecordReader for local training on these files, if required </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void createFileBatchesLocal(File inputDirectory, String[] extensions, boolean recursive, File outputDirectory, int batchSize) throws IOException
		Public Shared Sub createFileBatchesLocal(ByVal inputDirectory As File, ByVal extensions() As String, ByVal recursive As Boolean, ByVal outputDirectory As File, ByVal batchSize As Integer)
			If Not outputDirectory.exists() Then
				outputDirectory.mkdirs()
			End If
			'Local version
			Dim c As IList(Of File) = New List(Of File)(FileUtils.listFiles(inputDirectory, extensions, recursive))
			Collections.shuffle(c)

			'Construct file batch
			Dim list As IList(Of String) = New List(Of String)()
			Dim bytes As IList(Of SByte()) = New List(Of SByte())()
			For i As Integer = 0 To c.Count - 1
				list.Add(c(i).toURI().ToString())
				bytes.Add(FileUtils.readFileToByteArray(c(i)))

				If list.Count = batchSize Then
					process(list, bytes, outputDirectory)
				End If
			Next i
			If list.Count > 0 Then
				process(list, bytes, outputDirectory)
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void process(List<String> paths, List<byte[]> bytes, File outputDirectory) throws IOException
		Private Shared Sub process(ByVal paths As IList(Of String), ByVal bytes As IList(Of SByte()), ByVal outputDirectory As File)
			Dim fb As New FileBatch(bytes, paths)
			Dim name As String = System.Guid.randomUUID().ToString().replaceAll("-", "") & ".zip"
			Dim f As New File(outputDirectory, name)
			Using bos As New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
				fb.writeAsZip(bos)
			End Using

			paths.Clear()
			bytes.Clear()
		End Sub

		''' <summary>
		''' Create a number of <seealso cref="FileBatch"/> files from files on network storage such as HDFS (in random order).<br>
		''' Use cases: distributed training on compressed file formats such as images, that need to be loaded to a remote
		''' file storage system such as HDFS.<br>
		''' For example, if we were training with a minibatch size of 64 images, reading the raw images would result in 64
		''' different disk reads (one for each file) - which could clearly be a bottleneck during training.<br>
		''' Alternatively, we could create and save DataSet/INDArray objects containing a batch of images - however, storing
		''' images in FP32 (or ever UINT8) format - effectively a bitmap - is still much less efficient than the raw image files.<br>
		''' Instead, can create minibatches of <seealso cref="FileBatch"/> objects: these objects contain the raw file content for
		''' multiple files (as byte[]s) along with their original paths, which can then be used for distributed training using
		''' <seealso cref="RecordReaderFileBatchLoader"/>.<br>
		''' This approach gives us the benefits of the original file format (i.e., small size, compression) along with
		''' the benefits of a batched DataSet/INDArray format - i.e., disk reads are reduced by a factor of the minibatch size.<br>
		''' <br>
		''' See <seealso cref="createFileBatchesLocal(File, String[], Boolean, File, Integer)"/> for the local (non-Spark) version of this method.
		''' <br>
		''' Usage - image classification example - assume each FileBatch object contains a number of jpg/png etc image files
		''' <pre>
		''' {@code
		''' JavaSparkContext sc = ...
		''' SparkDl4jMultiLayer net = ...
		''' String baseFileBatchDir = ...
		''' JavaRDD<String> paths = org.deeplearning4j.spark.util.SparkUtils.listPaths(sc, baseFileBatchDir);
		''' 
		''' //Image record reader:
		''' PathLabelGenerator labelMaker = new ParentPathLabelGenerator();
		''' ImageRecordReader rr = new ImageRecordReader(32, 32, 1, labelMaker);
		''' rr.setLabels(<labels here>);
		''' 
		''' //Create DataSetLoader:
		''' int batchSize = 32;
		''' int numClasses = 1000;
		''' DataSetLoader loader = RecordReaderFileBatchLoader(rr, batchSize, 1, numClasses);
		''' 
		''' //Fit the network
		''' net.fitPaths(paths, loader);
		''' }
		''' </pre>
		''' </summary>
		''' <param name="batchSize"> Batch size - i.e., minibatch size to be used for training, and the number of files to
		'''                  include in each FileBatch object </param>
		''' <exception cref="IOException"> If an error occurs while reading the files </exception>
		''' <seealso cref= #createFileBatchesLocal(File, String[], boolean, File, int) </seealso>
		''' <seealso cref= org.datavec.api.records.reader.impl.filebatch.FileBatchRecordReader FileBatchRecordReader for local training on these files, if required </seealso>
		''' <seealso cref= org.datavec.api.records.reader.impl.filebatch.FileBatchSequenceRecordReader for local training on these files, if required </seealso>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static void createFileBatchesSpark(org.apache.spark.api.java.JavaRDD<String> filePaths, final String rootOutputDir, final int batchSize, org.apache.spark.api.java.JavaSparkContext sc)
		Public Shared Sub createFileBatchesSpark(ByVal filePaths As JavaRDD(Of String), ByVal rootOutputDir As String, ByVal batchSize As Integer, ByVal sc As JavaSparkContext)
			createFileBatchesSpark(filePaths, rootOutputDir, batchSize, sc.hadoopConfiguration())
		End Sub

		''' <summary>
		''' See <seealso cref="createFileBatchesSpark(JavaRDD, String, Integer, JavaSparkContext)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void createFileBatchesSpark(org.apache.spark.api.java.JavaRDD<String> filePaths, final String rootOutputDir, final int batchSize, @NonNull final org.apache.hadoop.conf.Configuration hadoopConfig)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Shared Sub createFileBatchesSpark(ByVal filePaths As JavaRDD(Of String), ByVal rootOutputDir As String, ByVal batchSize As Integer, ByVal hadoopConfig As org.apache.hadoop.conf.Configuration)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.datavec.spark.util.SerializableHadoopConfig conf = new org.datavec.spark.util.SerializableHadoopConfig(hadoopConfig);
			Dim conf As New SerializableHadoopConfig(hadoopConfig)
			'Here: assume input is images. We can't store them as Float32 arrays - that's too inefficient
			' instead: let's store the raw file content in a batch.
			Dim count As Long = filePaths.count()
			Dim maxPartitions As Long = count \ batchSize
			Dim repartitioned As JavaRDD(Of String) = filePaths.repartition(Math.Max(filePaths.getNumPartitions(), CInt(maxPartitions)))
			repartitioned.foreachPartition(New VoidFunctionAnonymousInnerClass(rootOutputDir, batchSize, conf))
		End Sub

		Private Class VoidFunctionAnonymousInnerClass
			Inherits VoidFunction(Of IEnumerator(Of String))

			Private rootOutputDir As String
			Private batchSize As Integer
			Private conf As SerializableHadoopConfig

			Public Sub New(ByVal rootOutputDir As String, ByVal batchSize As Integer, ByVal conf As SerializableHadoopConfig)
				Me.rootOutputDir = rootOutputDir
				Me.batchSize = batchSize
				Me.conf = conf
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(Iterator<String> stringIterator) throws Exception
			Public Overrides Sub [call](ByVal stringIterator As IEnumerator(Of String))
				'Construct file batch
				Dim list As IList(Of String) = New List(Of String)()
				Dim bytes As IList(Of SByte()) = New List(Of SByte())()
				Dim fs As FileSystem = FileSystem.get(conf.Configuration)
				Do While stringIterator.MoveNext()
					Dim inFile As String = stringIterator.Current
					Dim fileBytes() As SByte
					Using bis As New BufferedInputStream(fs.open(New org.apache.hadoop.fs.Path(inFile)))
						fileBytes = IOUtils.toByteArray(bis)
					End Using
					list.Add(inFile)
					bytes.Add(fileBytes)

					If list.Count = batchSize Then
						process(list, bytes)
					End If
				Loop
				If list.Count > 0 Then
					process(list, bytes)
				End If
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void process(List<String> paths, List<byte[]> bytes) throws IOException
			Private Sub process(ByVal paths As IList(Of String), ByVal bytes As IList(Of SByte()))
				Dim fb As New FileBatch(bytes, paths)
				Dim name As String = System.Guid.randomUUID().ToString().replaceAll("-", "") & ".zip"
				Dim outPath As String = FilenameUtils.concat(rootOutputDir, name)
				Dim fileSystem As FileSystem = FileSystem.get(conf.Configuration)
				Using bos As New BufferedOutputStream(fileSystem.create(New org.apache.hadoop.fs.Path(outPath)))
					fb.writeAsZip(bos)
				End Using

				paths.Clear()
				bytes.Clear()
			End Sub
		End Class

	End Class

End Namespace