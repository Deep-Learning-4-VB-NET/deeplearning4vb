Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports LocatedFileStatus = org.apache.hadoop.fs.LocatedFileStatus
Imports Path = org.apache.hadoop.fs.Path
Imports RemoteIterator = org.apache.hadoop.fs.RemoteIterator
Imports HashPartitioner = org.apache.spark.HashPartitioner
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Function2 = org.apache.spark.api.java.function.Function2
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports SerializerInstance = org.apache.spark.serializer.SerializerInstance
Imports Repartition = org.deeplearning4j.spark.api.Repartition
Imports RepartitionStrategy = org.deeplearning4j.spark.api.RepartitionStrategy
Imports BatchDataSetsFunction = org.deeplearning4j.spark.data.BatchDataSetsFunction
Imports SplitDataSetExamplesPairFlatMapFunction = org.deeplearning4j.spark.data.shuffle.SplitDataSetExamplesPairFlatMapFunction
Imports org.deeplearning4j.spark.impl.common
Imports org.deeplearning4j.spark.impl.common
Imports org.deeplearning4j.spark.impl.common
Imports BalancedPartitioner = org.deeplearning4j.spark.impl.common.repartition.BalancedPartitioner
Imports HashingBalancedPartitioner = org.deeplearning4j.spark.impl.common.repartition.HashingBalancedPartitioner
Imports org.deeplearning4j.spark.impl.common.repartition
Imports EqualRepartitioner = org.deeplearning4j.spark.impl.repartitioner.EqualRepartitioner
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports Tuple2 = scala.Tuple2

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkUtils
	Public Class SparkUtils

		Private Shared ReadOnly KRYO_EXCEPTION_MSG As String = "Kryo serialization detected without an appropriate registrator " & "for ND4J INDArrays." & vbLf & "When using Kryo, An appropriate Kryo registrator must be used to avoid" & " serialization issues (NullPointerException) with off-heap data in INDArrays." & vbLf & "Use nd4j-kryo_2.10 or _2.11 artifact, with sparkConf.set(""spark.kryo.registrator"", ""org.nd4j.kryo.Nd4jRegistrator"");" & vbLf & "See https://deeplearning4j.konduit.ai/distributed-deep-learning/howto#how-to-use-kryo-serialization-with-dl-4-j-and-nd-4-j for more details"

'JAVA TO VB CONVERTER NOTE: The field sparkExecutorId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared sparkExecutorId_Conflict As String

		Private Sub New()
		End Sub

		''' <summary>
		''' Check the spark configuration for incorrect Kryo configuration, logging a warning message if necessary
		''' </summary>
		''' <param name="javaSparkContext"> Spark context </param>
		''' <param name="log">              Logger to log messages to </param>
		''' <returns> True if ok (no kryo, or correct kryo setup) </returns>
		Public Shared Function checkKryoConfiguration(ByVal javaSparkContext As JavaSparkContext, ByVal log As Logger) As Boolean
			'Check if kryo configuration is correct:
			Dim serializer As String = javaSparkContext.getConf().get("spark.serializer", Nothing)
			If serializer IsNot Nothing AndAlso serializer.Equals("org.apache.spark.serializer.KryoSerializer") Then
				Dim kryoRegistrator As String = javaSparkContext.getConf().get("spark.kryo.registrator", Nothing)
				If kryoRegistrator Is Nothing OrElse Not kryoRegistrator.Equals("org.nd4j.kryo.Nd4jRegistrator") Then

					'It's probably going to fail later due to Kryo failing on the INDArray deserialization (off-heap data)
					'But: the user might be using a custom Kryo registrator that can handle ND4J INDArrays, even if they
					' aren't using the official ND4J-provided one
					'Either way: Let's test serialization now of INDArrays now, and fail early if necessary
					Dim si As SerializerInstance
					Dim bb As ByteBuffer
					Try
						si = javaSparkContext.env().serializer().newInstance()
						bb = si.serialize(Nd4j.linspace(1, 5, 5), Nothing)
					Catch e As Exception
						'Failed for some unknown reason during serialization - should never happen
						Throw New Exception(KRYO_EXCEPTION_MSG, e)
					End Try

					If bb Is Nothing Then
						'Should probably never happen
						Throw New Exception(KRYO_EXCEPTION_MSG & vbLf & "(Got: null ByteBuffer from Spark SerializerInstance)")
					Else
						'Could serialize successfully, but still may not be able to deserialize if kryo config is wrong
						Dim equals As Boolean
						Dim deserialized As INDArray
						Try
							deserialized = DirectCast(si.deserialize(bb, Nothing), INDArray)
							'Equals method may fail on malformed INDArrays, hence should be within the try-catch
							equals = Nd4j.linspace(1, 5, 5).Equals(deserialized)
						Catch e As Exception
							Throw New Exception(KRYO_EXCEPTION_MSG, e)
						End Try
						If Not equals Then
							Throw New Exception(KRYO_EXCEPTION_MSG & vbLf & "(Error during deserialization: test array" & " was not deserialized successfully)")
						End If

						'Otherwise: serialization/deserialization was successful using Kryo
						Return True
					End If
				End If
			End If
			Return True
		End Function

		''' <summary>
		''' Write a String to a file (on HDFS or local) in UTF-8 format
		''' </summary>
		''' <param name="path">    Path to write to </param>
		''' <param name="toWrite"> String to write </param>
		''' <param name="sc">      Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStringToFile(String path, String toWrite, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Sub writeStringToFile(ByVal path As String, ByVal toWrite As String, ByVal sc As JavaSparkContext)
			writeStringToFile(path, toWrite, sc.sc())
		End Sub

		''' <summary>
		''' Write a String to a file (on HDFS or local) in UTF-8 format
		''' </summary>
		''' <param name="path">    Path to write to </param>
		''' <param name="toWrite"> String to write </param>
		''' <param name="sc">      Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStringToFile(String path, String toWrite, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Sub writeStringToFile(ByVal path As String, ByVal toWrite As String, ByVal sc As SparkContext)
			Dim fileSystem As FileSystem = FileSystem.get(sc.hadoopConfiguration())
			Using bos As New BufferedOutputStream(fileSystem.create(New org.apache.hadoop.fs.Path(path)))
				bos.write(toWrite.GetBytes(Encoding.UTF8))
			End Using
		End Sub

		''' <summary>
		''' Read a UTF-8 format String from HDFS (or local)
		''' </summary>
		''' <param name="path"> Path to write the string </param>
		''' <param name="sc">   Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readStringFromFile(String path, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Function readStringFromFile(ByVal path As String, ByVal sc As JavaSparkContext) As String
			Return readStringFromFile(path, sc.sc())
		End Function

		''' <summary>
		''' Read a UTF-8 format String from HDFS (or local)
		''' </summary>
		''' <param name="path"> Path to write the string </param>
		''' <param name="sc">   Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readStringFromFile(String path, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Function readStringFromFile(ByVal path As String, ByVal sc As SparkContext) As String
			Dim fileSystem As FileSystem = FileSystem.get(sc.hadoopConfiguration())
			Using bis As New BufferedInputStream(fileSystem.open(New org.apache.hadoop.fs.Path(path)))
				Dim asBytes() As SByte = IOUtils.toByteArray(bis)
				Return StringHelper.NewString(asBytes, "UTF-8")
			End Using
		End Function

		''' <summary>
		''' Write an object to HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">    Path to write the object to </param>
		''' <param name="toWrite"> Object to write </param>
		''' <param name="sc">      Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeObjectToFile(String path, Object toWrite, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Sub writeObjectToFile(ByVal path As String, ByVal toWrite As Object, ByVal sc As JavaSparkContext)
			writeObjectToFile(path, toWrite, sc.sc())
		End Sub

		''' <summary>
		''' Write an object to HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">    Path to write the object to </param>
		''' <param name="toWrite"> Object to write </param>
		''' <param name="sc">      Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeObjectToFile(String path, Object toWrite, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Sub writeObjectToFile(ByVal path As String, ByVal toWrite As Object, ByVal sc As SparkContext)
			Dim fileSystem As FileSystem = FileSystem.get(sc.hadoopConfiguration())
			Using bos As New BufferedOutputStream(fileSystem.create(New org.apache.hadoop.fs.Path(path)))
				Dim oos As New ObjectOutputStream(bos)
				oos.writeObject(toWrite)
			End Using
		End Sub

		''' <summary>
		''' Read an object from HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path"> File to read </param>
		''' <param name="type"> Class of the object to read </param>
		''' <param name="sc">   Spark context </param>
		''' @param <T>  Type of the object to read </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T> T readObjectFromFile(String path, @Class<T> type, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Function readObjectFromFile(Of T)(ByVal path As String, ByVal type As Type(Of T), ByVal sc As JavaSparkContext) As T
			Return readObjectFromFile(path, type, sc.sc())
		End Function

		''' <summary>
		''' Read an object from HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path"> File to read </param>
		''' <param name="type"> Class of the object to read </param>
		''' <param name="sc">   Spark context </param>
		''' @param <T>  Type of the object to read </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T> T readObjectFromFile(String path, @Class<T> type, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Function readObjectFromFile(Of T)(ByVal path As String, ByVal type As Type(Of T), ByVal sc As SparkContext) As T
			Dim fileSystem As FileSystem = FileSystem.get(sc.hadoopConfiguration())
			Using ois As New ObjectInputStream(New BufferedInputStream(fileSystem.open(New org.apache.hadoop.fs.Path(path))))
				Dim o As Object
				Try
					o = ois.readObject()
				Catch e As ClassNotFoundException
					Throw New Exception(e)
				End Try

				Return DirectCast(o, T)
			End Using
		End Function

		''' <summary>
		''' Repartition the specified RDD (or not) using the given <seealso cref="Repartition"/> and <seealso cref="RepartitionStrategy"/> settings
		''' </summary>
		''' <param name="rdd">                 RDD to repartition </param>
		''' <param name="repartition">         Setting for when repartiting is to be conducted </param>
		''' <param name="repartitionStrategy"> Setting for how repartitioning is to be conducted </param>
		''' <param name="objectsPerPartition"> Desired number of objects per partition </param>
		''' <param name="numPartitions">       Total number of partitions </param>
		''' @param <T>                 Type of the RDD </param>
		''' <returns> Repartitioned RDD, or original RDD if no repartitioning was conducted </returns>
'JAVA TO VB CONVERTER NOTE: The parameter repartition was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Shared Function repartition(Of T)(ByVal rdd As JavaRDD(Of T), ByVal repartition_Conflict As Repartition, ByVal repartitionStrategy As RepartitionStrategy, ByVal objectsPerPartition As Integer, ByVal numPartitions As Integer) As JavaRDD(Of T)
			If repartition_Conflict = Repartition.Never Then
				Return rdd
			End If

			Select Case repartitionStrategy
				Case RepartitionStrategy.SparkDefault
					If repartition_Conflict = Repartition.NumPartitionsWorkersDiffers AndAlso rdd.partitions().size() = numPartitions Then
						Return rdd
					End If

					'Either repartition always, or workers/num partitions differs
					Return rdd.repartition(numPartitions)
				Case RepartitionStrategy.Balanced
					Return repartitionBalanceIfRequired(rdd, repartition_Conflict, objectsPerPartition, numPartitions)
				Case RepartitionStrategy.ApproximateBalanced
					Return repartitionApproximateBalance(rdd, repartition_Conflict, numPartitions)
				Case Else
					Throw New Exception("Unknown repartition strategy: " & repartitionStrategy)
			End Select
		End Function

		Public Shared Function repartitionApproximateBalance(Of T)(ByVal rdd As JavaRDD(Of T), ByVal repartition As Repartition, ByVal numPartitions As Integer) As JavaRDD(Of T)
			Dim origNumPartitions As Integer = rdd.partitions().size()
			Select Case repartition
				Case Repartition.Never
					Return rdd
				Case Repartition.NumPartitionsWorkersDiffers
					If origNumPartitions = numPartitions Then
						Return rdd
					End If
				Case Repartition.Always
					' Count each partition...
					Dim partitionCounts As IList(Of Integer) = rdd.mapPartitionsWithIndex(New Function2AnonymousInnerClass()
								   , True).collect()

					Dim totalCount As Integer? = 0
					For Each i As Integer? In partitionCounts
						totalCount += i
					Next i
					Dim partitionWeights As IList(Of Double) = New List(Of Double)(Math.Max(numPartitions, origNumPartitions))
					Dim ideal As Double? = CDbl(totalCount) / numPartitions
					' partitions in the initial set and not in the final one get -1 => elements always jump
					' partitions in the final set not in the initial one get 0 => aim to receive the average amount
					Dim i As Integer = 0
					Do While i < Math.Min(origNumPartitions, numPartitions)
						partitionWeights.Add(CDbl(partitionCounts(i)) / ideal.Value)
						i += 1
					Loop
					i = Math.Min(origNumPartitions, numPartitions)
					Do While i < Math.Max(origNumPartitions, numPartitions)
						' we shrink the # of partitions
						If i >= numPartitions Then
							partitionWeights.Add(-1R)
						' we enlarge the # of partitions
						Else
							partitionWeights.Add(0R)
						End If
						i += 1
					Loop

					' this method won't trigger a spark job, which is different from {@link org.apache.spark.rdd.RDD#zipWithIndex}

					Dim indexedRDD As JavaPairRDD(Of Tuple2(Of Long, Integer), T) = rdd.zipWithUniqueId().mapToPair(New PairFunctionAnonymousInnerClass())

					Dim hbp As New HashingBalancedPartitioner(Collections.singletonList(partitionWeights))
					Dim partitionedRDD As JavaPairRDD(Of Tuple2(Of Long, Integer), T) = indexedRDD.partitionBy(hbp)

					Return partitionedRDD.map(New FunctionAnonymousInnerClass())
				Case Else
					Throw New Exception("Unknown setting for repartition: " & repartition)
			End Select
		End Function

		Private Class Function2AnonymousInnerClass
			Inherits Function2(Of Integer, IEnumerator(Of T), IEnumerator(Of Integer))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public Iterator<Integer> call(System.Nullable<Integer> integer, Iterator<T> tIterator) throws Exception
			Public Overrides Function [call](ByVal [integer] As Integer?, ByVal tIterator As IEnumerator(Of T)) As IEnumerator(Of Integer)
				Dim count As Integer = 0
				Do While tIterator.MoveNext()
					tIterator.Current
					count += 1
				Loop
				Return Collections.singletonList(count).GetEnumerator()
			End Function
		End Class

		Private Class PairFunctionAnonymousInnerClass
			Inherits PairFunction(Of Tuple2(Of T, Long), Tuple2(Of Long, Integer), T)

			Public Overrides Function [call](ByVal tLongTuple2 As Tuple2(Of T, Long)) As Tuple2(Of Tuple2(Of Long, Integer), T)
				Return New Tuple2(Of Tuple2(Of Long, Integer), T)(New Tuple2(Of Long, Integer)(tLongTuple2._2(), 0), tLongTuple2._1())
			End Function
		End Class

		Private Class FunctionAnonymousInnerClass
			Inherits [Function](Of Tuple2(Of Tuple2(Of Long, Integer), T), T)

			Public Overrides Function [call](ByVal indexNPayload As Tuple2(Of Tuple2(Of Long, Integer), T)) As T
				Return indexNPayload._2()
			End Function
		End Class

		''' <summary>
		''' Repartition a RDD (given the <seealso cref="Repartition"/> setting) such that we have approximately
		''' {@code numPartitions} partitions, each of which has {@code objectsPerPartition} objects.
		''' </summary>
		''' <param name="rdd"> RDD to repartition </param>
		''' <param name="repartition"> Repartitioning setting </param>
		''' <param name="objectsPerPartition"> Number of objects we want in each partition </param>
		''' <param name="numPartitions"> Number of partitions to have </param>
		''' @param <T> Type of RDD </param>
		''' <returns> Repartitioned RDD, or the original RDD if no repartitioning was performed </returns>
		Public Shared Function repartitionBalanceIfRequired(Of T)(ByVal rdd As JavaRDD(Of T), ByVal repartition As Repartition, ByVal objectsPerPartition As Integer, ByVal numPartitions As Integer) As JavaRDD(Of T)
			Dim origNumPartitions As Integer = rdd.partitions().size()
			Select Case repartition
				Case Repartition.Never
					Return rdd
				Case Repartition.NumPartitionsWorkersDiffers
					If origNumPartitions = numPartitions Then
						Return rdd
					End If
				Case Repartition.Always
					'Repartition: either always, or origNumPartitions != numWorkers

					'First: count number of elements in each partition. Need to know this so we can work out how to properly index each example,
					' so we can in turn create properly balanced partitions after repartitioning
					'Because the objects (DataSets etc) should be small, this should be OK

					'Count each partition...
					Dim partitionCounts As IList(Of Tuple2(Of Integer, Integer)) = rdd.mapPartitionsWithIndex(New CountPartitionsFunction(Of T)(), True).collect()
					Dim totalObjects As Integer = 0
					Dim initialPartitions As Integer = partitionCounts.Count

					Dim allCorrectSize As Boolean = True
					Dim x As Integer = 0
					For Each t2 As Tuple2(Of Integer, Integer) In partitionCounts
						Dim partitionSize As Integer = t2._2()
						allCorrectSize = allCorrectSize And (partitionSize = objectsPerPartition)
						totalObjects += t2._2()
					Next t2

					If numPartitions * objectsPerPartition < totalObjects Then
						allCorrectSize = True
						For Each t2 As Tuple2(Of Integer, Integer) In partitionCounts
							allCorrectSize = allCorrectSize And (t2._2() = objectsPerPartition)
						Next t2
					End If

					If initialPartitions = numPartitions AndAlso allCorrectSize Then
						'Don't need to do any repartitioning here - already in the format we want
						Return rdd
					End If

					'Index each element for repartitioning (can only do manual repartitioning on a JavaPairRDD)
					Dim pairIndexed As JavaPairRDD(Of Integer, T) = indexedRDD(rdd)

					Dim remainder As Integer = (totalObjects - numPartitions * objectsPerPartition) Mod numPartitions
					log.trace("About to rebalance: numPartitions={}, objectsPerPartition={}, remainder={}", numPartitions, objectsPerPartition, remainder)
					pairIndexed = pairIndexed.partitionBy(New BalancedPartitioner(numPartitions, objectsPerPartition, remainder))
					Return pairIndexed.values()
				Case Else
					Throw New Exception("Unknown setting for repartition: " & repartition)
			End Select
		End Function

		Public Shared Function indexedRDD(Of T)(ByVal rdd As JavaRDD(Of T)) As JavaPairRDD(Of Integer, T)
			Return rdd.zipWithIndex().mapToPair(New PairFunctionAnonymousInnerClass2())
		End Function

		Private Class PairFunctionAnonymousInnerClass2
			Inherits PairFunction(Of Tuple2(Of T, Long), Integer, T)

			Public Overrides Function [call](ByVal elemIdx As Tuple2(Of T, Long)) As Tuple2(Of Integer, T)
				Return New Tuple2(Of Integer, T)(elemIdx._2().intValue(), elemIdx._1())
			End Function
		End Class

		Public Shared Function repartitionEqually(Of T)(ByVal rdd As JavaRDD(Of T), ByVal repartition As Repartition, ByVal numPartitions As Integer) As JavaRDD(Of T)
			Dim origNumPartitions As Integer = rdd.partitions().size()
			Select Case repartition
				Case Repartition.Never
					Return rdd
				Case Repartition.NumPartitionsWorkersDiffers
					If origNumPartitions = numPartitions Then
						Return rdd
					End If
				Case Repartition.Always
					Return (New EqualRepartitioner()).repartition(rdd, -1, numPartitions)
				Case Else
					Throw New Exception("Unknown setting for repartition: " & repartition)
			End Select
		End Function

		''' <summary>
		''' Random split the specified RDD into a number of RDDs, where each has {@code numObjectsPerSplit} in them.
		''' <para>
		''' This similar to how RDD.randomSplit works (i.e., split via filtering), but this should result in more
		''' equal splits (instead of independent binomial sampling that is used there, based on weighting)
		''' This balanced splitting approach is important when the number of DataSet objects we want in each split is small,
		''' as random sampling variance of <seealso cref="JavaRDD.randomSplit(Double[])"/> is quite large relative to the number of examples
		''' in each split. Note however that this method doesn't <i>guarantee</i> that partitions will be balanced
		''' </para>
		''' <para>
		''' Downside is we need total object count (whereas <seealso cref="JavaRDD.randomSplit(Double[])"/> does not). However, randomSplit
		''' requires a full pass of the data anyway (in order to do filtering upon it) so this should not add much overhead in practice
		''' 
		''' </para>
		''' </summary>
		''' <param name="totalObjectCount">   Total number of objects in the RDD to split </param>
		''' <param name="numObjectsPerSplit"> Number of objects in each split </param>
		''' <param name="data">               Data to split </param>
		''' @param <T>                Generic type for the RDD </param>
		''' <returns> The RDD split up (without replacement) into a number of smaller RDDs </returns>
		Public Shared Function balancedRandomSplit(Of T)(ByVal totalObjectCount As Integer, ByVal numObjectsPerSplit As Integer, ByVal data As JavaRDD(Of T)) As JavaRDD(Of T)()
			Return balancedRandomSplit(totalObjectCount, numObjectsPerSplit, data, (New Random()).nextLong())
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="balancedRandomSplit(Integer, Integer, JavaRDD)"/> with control over the RNG seed
		''' </summary>
		Public Shared Function balancedRandomSplit(Of T)(ByVal totalObjectCount As Integer, ByVal numObjectsPerSplit As Integer, ByVal data As JavaRDD(Of T), ByVal rngSeed As Long) As JavaRDD(Of T)()
			Dim splits() As JavaRDD(Of T)
			If totalObjectCount <= numObjectsPerSplit Then
				splits = CType(Array.CreateInstance(GetType(JavaRDD), 1), JavaRDD(Of T)())
				splits(0) = data
			Else
				Dim numSplits As Integer = totalObjectCount \ numObjectsPerSplit 'Intentional round down
				splits = CType(Array.CreateInstance(GetType(JavaRDD), numSplits), JavaRDD(Of T)())
				Dim i As Integer = 0
				Do While i < numSplits
					splits(i) = data.mapPartitionsWithIndex(New SplitPartitionsFunction(Of T)(i, numSplits, rngSeed), True)
					i += 1
				Loop

			End If
			Return splits
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="balancedRandomSplit(Integer, Integer, JavaRDD)"/> but for Pair RDDs
		''' </summary>
		Public Shared Function balancedRandomSplit(Of T, U)(ByVal totalObjectCount As Integer, ByVal numObjectsPerSplit As Integer, ByVal data As JavaPairRDD(Of T, U)) As JavaPairRDD(Of T, U)()
			Return balancedRandomSplit(totalObjectCount, numObjectsPerSplit, data, (New Random()).nextLong())
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="balancedRandomSplit(Integer, Integer, JavaRDD)"/> but for pair RDDs, and with control over the RNG seed
		''' </summary>
		Public Shared Function balancedRandomSplit(Of T, U)(ByVal totalObjectCount As Integer, ByVal numObjectsPerSplit As Integer, ByVal data As JavaPairRDD(Of T, U), ByVal rngSeed As Long) As JavaPairRDD(Of T, U)()
			Dim splits() As JavaPairRDD(Of T, U)
			If totalObjectCount <= numObjectsPerSplit Then
				splits = CType(Array.CreateInstance(GetType(JavaPairRDD), 1), JavaPairRDD(Of T, U)())
				splits(0) = data
			Else
				Dim numSplits As Integer = totalObjectCount \ numObjectsPerSplit 'Intentional round down

				splits = CType(Array.CreateInstance(GetType(JavaPairRDD), numSplits), JavaPairRDD(Of T, U)())
				Dim i As Integer = 0
				Do While i < numSplits

					'What we really need is a .mapPartitionsToPairWithIndex function
					'but, of course Spark doesn't provide this
					'So we need to do a two-step process here...

					Dim split As JavaRDD(Of Tuple2(Of T, U)) = data.mapPartitionsWithIndex(New SplitPartitionsFunction2(Of T, U)(i, numSplits, rngSeed), True)
					splits(i) = split.mapPartitionsToPair(New MapTupleToPairFlatMap(Of T, U)(), True)
					i += 1
				Loop
			End If
			Return splits
		End Function

		''' <summary>
		''' List of the files in the given directory (path), as a {@code JavaRDD<String>}
		''' </summary>
		''' <param name="sc">      Spark context </param>
		''' <param name="path">    Path to list files in </param>
		''' <returns>        Paths in the directory </returns>
		''' <exception cref="IOException"> If error occurs getting directory contents </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<String> listPaths(org.apache.spark.api.java.JavaSparkContext sc, String path) throws IOException
		Public Shared Function listPaths(ByVal sc As JavaSparkContext, ByVal path As String) As JavaRDD(Of String)
			Return listPaths(sc, path, False)
		End Function

		''' <summary>
		''' List of the files in the given directory (path), as a {@code JavaRDD<String>}
		''' </summary>
		''' <param name="sc">        Spark context </param>
		''' <param name="path">      Path to list files in </param>
		''' <param name="recursive"> Whether to walk the directory tree recursively (i.e., include subdirectories) </param>
		''' <returns> Paths in the directory </returns>
		''' <exception cref="IOException"> If error occurs getting directory contents </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<String> listPaths(org.apache.spark.api.java.JavaSparkContext sc, String path, boolean recursive) throws IOException
		Public Shared Function listPaths(ByVal sc As JavaSparkContext, ByVal path As String, ByVal recursive As Boolean) As JavaRDD(Of String)
			'NativeImageLoader.ALLOWED_FORMATS
			Return listPaths(sc, path, recursive, DirectCast(Nothing, ISet(Of String)))
		End Function

		''' <summary>
		''' List of the files in the given directory (path), as a {@code JavaRDD<String>}
		''' </summary>
		''' <param name="sc">                Spark context </param>
		''' <param name="path">              Path to list files in </param>
		''' <param name="recursive">         Whether to walk the directory tree recursively (i.e., include subdirectories) </param>
		''' <param name="allowedExtensions"> If null: all files will be accepted. If non-null: only files with the specified extension will be allowed.
		'''                          Exclude the extension separator - i.e., use "txt" not ".txt" here. </param>
		''' <returns> Paths in the directory </returns>
		''' <exception cref="IOException"> If error occurs getting directory contents </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<String> listPaths(org.apache.spark.api.java.JavaSparkContext sc, String path, boolean recursive, String[] allowedExtensions) throws IOException
		Public Shared Function listPaths(ByVal sc As JavaSparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal allowedExtensions() As String) As JavaRDD(Of String)
			Return listPaths(sc, path, recursive, (If(allowedExtensions Is Nothing, Nothing, New HashSet(Of )(java.util.Arrays.asList(allowedExtensions)))))
		End Function

		''' <summary>
		''' List of the files in the given directory (path), as a {@code JavaRDD<String>}
		''' </summary>
		''' <param name="sc">                Spark context </param>
		''' <param name="path">              Path to list files in </param>
		''' <param name="recursive">         Whether to walk the directory tree recursively (i.e., include subdirectories) </param>
		''' <param name="allowedExtensions"> If null: all files will be accepted. If non-null: only files with the specified extension will be allowed.
		'''                          Exclude the extension separator - i.e., use "txt" not ".txt" here. </param>
		''' <returns> Paths in the directory </returns>
		''' <exception cref="IOException"> If error occurs getting directory contents </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<String> listPaths(org.apache.spark.api.java.JavaSparkContext sc, String path, boolean recursive, @Set<String> allowedExtensions) throws IOException
		Public Shared Function listPaths(ByVal sc As JavaSparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal allowedExtensions As ISet(Of String)) As JavaRDD(Of String)
			Return listPaths(sc, path, recursive, allowedExtensions, sc.hadoopConfiguration())
		End Function

		''' <summary>
		''' List of the files in the given directory (path), as a {@code JavaRDD<String>}
		''' </summary>
		''' <param name="sc">                Spark context </param>
		''' <param name="path">              Path to list files in </param>
		''' <param name="recursive">         Whether to walk the directory tree recursively (i.e., include subdirectories) </param>
		''' <param name="allowedExtensions"> If null: all files will be accepted. If non-null: only files with the specified extension will be allowed.
		'''                          Exclude the extension separator - i.e., use "txt" not ".txt" here. </param>
		''' <param name="config">            Hadoop configuration to use. Must not be null. </param>
		''' <returns> Paths in the directory </returns>
		''' <exception cref="IOException"> If error occurs getting directory contents </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.apache.spark.api.java.JavaRDD<String> listPaths(@NonNull JavaSparkContext sc, String path, boolean recursive, @Set<String> allowedExtensions, @NonNull Configuration config) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function listPaths(ByVal sc As JavaSparkContext, ByVal path As String, ByVal recursive As Boolean, ByVal allowedExtensions As ISet(Of String), ByVal config As Configuration) As JavaRDD(Of String)
			Dim paths As IList(Of String) = New List(Of String)()
			Dim hdfs As FileSystem = FileSystem.get(URI.create(path), config)
			Dim fileIter As RemoteIterator(Of LocatedFileStatus) = hdfs.listFiles(New Path(path), recursive)

			Do While fileIter.hasNext()
				Dim filePath As String = fileIter.next().getPath().ToString()
				If allowedExtensions Is Nothing Then
					paths.Add(filePath)
				Else
					Dim ext As String = FilenameUtils.getExtension(path)
					If allowedExtensions.Contains(ext) Then
						paths.Add(filePath)
					End If
				End If
			Loop
			Return sc.parallelize(paths)
		End Function


		''' <summary>
		''' Randomly shuffle the examples in each DataSet object, and recombine them into new DataSet objects
		''' with the specified BatchSize
		''' </summary>
		''' <param name="rdd"> DataSets to shuffle/recombine </param>
		''' <param name="newBatchSize"> New batch size for the DataSet objects, after shuffling/recombining </param>
		''' <param name="numPartitions"> Number of partitions to use when splitting/recombining </param>
		''' <returns> A new <seealso cref="JavaRDD<DataSet>"/>, with the examples shuffled/combined in each </returns>
		Public Shared Function shuffleExamples(ByVal rdd As JavaRDD(Of DataSet), ByVal newBatchSize As Integer, ByVal numPartitions As Integer) As JavaRDD(Of DataSet)
			'Step 1: split into individual examples, mapping to a pair RDD (random key in range 0 to numPartitions)

			Dim singleExampleDataSets As JavaPairRDD(Of Integer, DataSet) = rdd.flatMapToPair(New SplitDataSetExamplesPairFlatMapFunction(numPartitions))

			'Step 2: repartition according to the random keys
			singleExampleDataSets = singleExampleDataSets.partitionBy(New HashPartitioner(numPartitions))

			'Step 3: Recombine
			Return singleExampleDataSets.values().mapPartitions(New BatchDataSetsFunction(newBatchSize))
		End Function

		''' <summary>
		''' Get the Spark executor ID<br>
		''' The ID is parsed from the JVM launch args. If that is not specified (or can't be obtained) then the value
		''' from <seealso cref="UIDProvider.getJVMUID()"/> is returned
		''' @return
		''' </summary>
		Public Shared ReadOnly Property SparkExecutorId As String
			Get
				If sparkExecutorId_Conflict IsNot Nothing Then
					Return sparkExecutorId_Conflict
				End If
    
				SyncLock GetType(SparkUtils)
					're-check, in case some other thread set it while waiting for lock
					If sparkExecutorId_Conflict IsNot Nothing Then
						Return sparkExecutorId_Conflict
					End If
    
					Dim s As String = System.getProperty("sun.java.command")
					If s Is Nothing OrElse s.Length = 0 OrElse Not s.Contains("executor-id") Then
						sparkExecutorId_Conflict = UIDProvider.JVMUID
						Return sparkExecutorId_Conflict
					End If
    
					Dim idx As Integer = s.IndexOf("executor-id", StringComparison.Ordinal)
					Dim [sub] As String = s.Substring(idx)
					Dim split() As String = [sub].Split(" ", True)
					If split.Length < 2 Then
						sparkExecutorId_Conflict = UIDProvider.JVMUID
						Return sparkExecutorId_Conflict
					End If
					sparkExecutorId_Conflict = split(1)
					Return sparkExecutorId_Conflict
				End SyncLock
			End Get
		End Property

		Public Shared Function asByteArrayBroadcast(ByVal sc As JavaSparkContext, ByVal array As INDArray) As Broadcast(Of SByte())
			Dim baos As New MemoryStream()
			Try
				Nd4j.write(array, New DataOutputStream(baos))
			Catch e As IOException
				Throw New Exception(e) 'Should never happen
			End Try
			Dim paramBytes() As SByte = baos.toByteArray() 'See docs in EvaluationRunner for why we use byte[] instead of INDArray (thread locality etc)
			Return sc.broadcast(paramBytes)
		End Function
	End Class

End Namespace