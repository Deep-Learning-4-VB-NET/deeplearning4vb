Imports System
Imports System.Collections.Generic
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext

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

Namespace org.deeplearning4j.spark


	<Serializable>
	Public Class BaseSparkKryoTest
		Inherits BaseSparkTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property

		Public Overrides ReadOnly Property Context As JavaSparkContext
			Get
				If sc IsNot Nothing Then
					Return sc
				End If
    
				'Ensure SPARK_USER environment variable is set for Spark Kryo tests
				Dim u As String = Environment.GetEnvironmentVariable("SPARK_USER")
				If u Is Nothing OrElse u.Length = 0 Then
					Try
						Dim classes() As Type = GetType(Collections).GetNestedTypes(BindingFlags.Public Or BindingFlags.NonPublic)
						Dim env As IDictionary(Of String, String) = System.getenv()
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
    
    
    
				Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[" & numExecutors() & "]").setAppName("sparktest").set("spark.driver.host", "localhost")
    
				sparkConf.set("spark.serializer", "org.apache.spark.serializer.KryoSerializer")
				sparkConf.set("spark.kryo.registrator", "org.nd4j.kryo.Nd4jRegistrator")
    
				sc = New JavaSparkContext(sparkConf)
    
				Return sc
			End Get
		End Property

	End Class


End Namespace