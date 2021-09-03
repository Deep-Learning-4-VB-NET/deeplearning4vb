Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Platform = com.sun.jna.Platform
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports SerializerInstance = org.apache.spark.serializer.SerializerInstance
Imports org.junit.jupiter.api
Imports org.nd4j.common.primitives
Imports Downloader = org.nd4j.common.resources.Downloader
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Tuple2 = scala.Tuple2
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestNd4jKryoSerialization extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestNd4jKryoSerialization
		Inherits BaseND4JTest

		Private sc As JavaSparkContext

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll @SneakyThrows public static void beforeAll()
		Public Shared Sub beforeAll()
			If Platform.isWindows() Then
				Dim hadoopHome As New File(System.getProperty("java.io.tmpdir"),"hadoop-tmp")
				Dim binDir As New File(hadoopHome,"bin")
				If Not binDir.exists() Then
					binDir.mkdirs()
				End If
				Dim outputFile As New File(binDir,"winutils.exe")
				If Not outputFile.exists() Then
					log.info("Fixing spark for windows")
					Downloader.download("winutils.exe", URI.create("https://github.com/cdarlint/winutils/blob/master/hadoop-2.6.5/bin/winutils.exe?raw=true").toURL(), outputFile,"db24b404d2331a1bec7443336a5171f1",3)
				End If

				System.setProperty("hadoop.home.dir", hadoopHome.getAbsolutePath())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Dim sparkConf As New SparkConf()
			sparkConf.setMaster("local[*]")
			sparkConf.set("spark.driver.host", "localhost")
			sparkConf.setAppName("Iris")

			sparkConf.set("spark.serializer", "org.apache.spark.serializer.KryoSerializer")
			sparkConf.set("spark.kryo.registrator", "org.nd4j.kryo.Nd4jRegistrator")

			sc = New JavaSparkContext(sparkConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerialization()
		Public Overridable Sub testSerialization()

			Dim t2 As New Tuple2(Of INDArray, INDArray)(Nd4j.linspace(1, 10, 10, DataType.FLOAT), Nd4j.linspace(10, 20, 10, DataType.FLOAT))

			Dim b As Broadcast(Of Tuple2(Of INDArray, INDArray)) = sc.broadcast(t2)

			Dim list As IList(Of INDArray) = New List(Of INDArray)()
			For i As Integer = 0 To 99
				list.Add(Nd4j.ones(5))
			Next i

			Dim rdd As JavaRDD(Of INDArray) = sc.parallelize(list)

			rdd.foreach(New AssertFn(b))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerializationPrimitives()
		Public Overridable Sub testSerializationPrimitives()

			Dim c As New Counter(Of Integer)()
			c.incrementCount(5, 3.0)

			Dim cm As New CounterMap(Of Integer, Double)()
			cm.setCount(7, 3.0, 4.5)

			Dim objs() As Object = {
				New AtomicBoolean(True),
				New AtomicBoolean(False),
				New AtomicDouble(5.0),
				c,
				cm,
				New ImmutablePair(Of )(5,3.0),
				New ImmutableQuad(Of )(1,2.0,3.0f,4L),
				New ImmutableTriple(Of )(1,2.0,3.0f),
				New Pair(Of )(5, 3.0),
				New Quad(Of )(1,2.0,3.0f,4L),
				New Triple(Of )(1,2.0,3.0f)
			}


			Dim si As SerializerInstance = sc.env().serializer().newInstance()

			For Each o As Object In objs
				Console.WriteLine(o.GetType())
				'System.out.println(ie.getClass());
				testSerialization(o, si)
			Next o
		End Sub

		Private Sub testSerialization(Of T)(ByVal [in] As T, ByVal si As SerializerInstance)
			Dim bb As ByteBuffer = si.serialize([in], Nothing)
			Dim deserialized As T = CType(si.deserialize(bb, Nothing), T)

	'        assertEquals(in, deserialized);
			Dim equals As Boolean = [in].Equals(deserialized)
			assertTrue(equals,[in].GetType() & vbTab & [in].ToString())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			If sc IsNot Nothing Then
				sc.close()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public static class AssertFn implements org.apache.spark.api.java.function.VoidFunction<org.nd4j.linalg.api.ndarray.INDArray>
		Public Class AssertFn
			Implements VoidFunction(Of INDArray)

			Friend b As Broadcast(Of Tuple2(Of INDArray, INDArray))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(org.nd4j.linalg.api.ndarray.INDArray arr) throws Exception
			Public Overrides Sub [call](ByVal arr As INDArray)
				Dim t2 As Tuple2(Of INDArray, INDArray) = b.getValue()
				assertEquals(Nd4j.linspace(1, 10, 10, DataType.FLOAT), t2._1())
				assertEquals(Nd4j.linspace(10, 20, 10, DataType.FLOAT), t2._2())

				assertEquals(Nd4j.ones(5), arr)
			End Sub
		End Class
	End Class

End Namespace