Imports PairFunction = org.apache.spark.api.java.function.PairFunction
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
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

	Public Class PathToKeyFunction
		Implements PairFunction(Of Tuple2(Of String, PortableDataStream), String, Tuple3(Of String, Integer, PortableDataStream))

		Private converter As PathToKeyConverter
		Private index As Integer

		Public Sub New(ByVal index As Integer, ByVal converter As PathToKeyConverter)
			Me.index = index
			Me.converter = converter
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public scala.Tuple2<String, scala.Tuple3<String, Integer, org.apache.spark.input.PortableDataStream>> call(scala.Tuple2<String, org.apache.spark.input.PortableDataStream> in) throws Exception
		Public Overrides Function [call](ByVal [in] As Tuple2(Of String, PortableDataStream)) As Tuple2(Of String, Tuple3(Of String, Integer, PortableDataStream))
			Dim [out] As New Tuple3(Of String, Integer, PortableDataStream)([in]._1(), index, [in]._2())
			Dim newKey As String = converter.getKey([in]._1())
			Return New Tuple2(Of String, Tuple3(Of String, Integer, PortableDataStream))(newKey, [out])
		End Function
	End Class

End Namespace