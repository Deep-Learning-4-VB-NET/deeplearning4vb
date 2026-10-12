Imports System.Collections.Generic
Imports Text = org.apache.hadoop.io.Text
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports DataVecSparkUtil = org.datavec.spark.util.DataVecSparkUtil
Imports Tuple2 = scala.Tuple2
Imports Tuple3 = scala.Tuple3

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

Namespace org.datavec.spark.functions.pairdata

	Public Class MapToBytesPairWritableFunction
		Implements PairFunction(Of Tuple2(Of String, IEnumerable(Of Tuple3(Of String, Integer, PortableDataStream))), Text, BytesPairWritable)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<org.apache.hadoop.io.Text, BytesPairWritable> call(scala.Tuple2<String, Iterable<scala.Tuple3<String, Integer, org.apache.spark.input.PortableDataStream>>> in) throws Exception
		Public Overrides Function [call](ByVal [in] As Tuple2(Of String, IEnumerable(Of Tuple3(Of String, Integer, PortableDataStream)))) As Tuple2(Of Text, BytesPairWritable)
			Dim first() As SByte = Nothing
			Dim second() As SByte = Nothing
			Dim firstOrigPath As String = Nothing
			Dim secondOrigPath As String = Nothing
			Dim iterable As IEnumerable(Of Tuple3(Of String, Integer, PortableDataStream)) = [in]._2()
			For Each tuple As Tuple3(Of String, Integer, PortableDataStream) In iterable
				If tuple._2() = 0 Then
					first = tuple._3().toArray()
					firstOrigPath = tuple._1()
				ElseIf tuple._2() = 1 Then
					second = tuple._3().toArray()
					secondOrigPath = tuple._1()
				End If
			Next tuple
			Return New Tuple2(Of Text, BytesPairWritable)(New Text([in]._1()), New BytesPairWritable(first, second, firstOrigPath, secondOrigPath))
		End Function
	End Class

End Namespace