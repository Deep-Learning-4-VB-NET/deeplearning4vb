Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Word2VecVariables = org.deeplearning4j.spark.models.embeddings.word2vec.Word2VecVariables
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Downloader = org.nd4j.common.resources.Downloader

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

Namespace org.deeplearning4j.spark.text

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseSparkTest extends org.deeplearning4j.BaseDL4JTest implements java.io.Serializable
	<Serializable>
	Public MustInherit Class BaseSparkTest
		Inherits BaseDL4JTest

		<NonSerialized>
		Protected Friend sc As JavaSparkContext

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



		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub before()
			sc = Context
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			If sc IsNot Nothing Then
				sc.close()
			End If
			sc = Nothing
		End Sub

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Context As JavaSparkContext
			Get
				If sc IsNot Nothing Then
					Return sc
				End If
    
				'Ensure SPARK_USER environment variable is set for Spark tests
				Dim u As String = Environment.GetEnvironmentVariable("SPARK_USER")
				Dim env As IDictionary(Of String, String) = System.getenv()
				If u Is Nothing OrElse u.Length = 0 Then
					Try
						Dim classes() As Type = GetType(Collections).GetNestedTypes(BindingFlags.Public Or BindingFlags.NonPublic)
						For Each cl As Type In classes
	'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							If "java.util.Collections$UnmodifiableMap".Equals(cl.FullName) Then
								Dim field As System.Reflection.FieldInfo = cl.getDeclaredField("m")
								field.setAccessible(True)
								Dim obj As Object = field.get(env)
								Dim map As IDictionary(Of String, String) = DirectCast(obj, IDictionary(Of String, String))
								Dim user As String = System.getProperty("user.name")
								If user Is Nothing OrElse user.Length = 0 Then
									user = "user"
								End If
								map("SPARK_USER") = user
							End If
						Next cl
					Catch e As Exception
						Throw New Exception(e)
					End Try
				End If
    
				' set to test mode
				Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[4]").set("spark.driver.host", "localhost").setAppName("sparktest").set(Word2VecVariables.NUM_WORDS, 1.ToString())
    
    
				sc = New JavaSparkContext(sparkConf)
				Return sc
    
			End Get
		End Property

	End Class

End Namespace