Imports System.Collections.Generic
Imports TimeSeriesWritableUtils = org.datavec.api.timeseries.util.TimeSeriesWritableUtils
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Writable = org.datavec.api.writable.Writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.junit.jupiter.api.Assertions.assertArrayEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.datavec.api.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Time Series Utils Test") @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class TimeSeriesUtilsTest extends org.nd4j.common.tests.BaseND4JTest
	Friend Class TimeSeriesUtilsTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Time Series Creation") void testTimeSeriesCreation()
		Friend Overridable Sub testTimeSeriesCreation()
			Dim test As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim timeStep As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For i As Integer = 0 To 4
				timeStep.Add(getRecord(5))
			Next i
			test.Add(timeStep)
			Dim arr As INDArray = TimeSeriesWritableUtils.convertWritablesSequence(test).First
			assertArrayEquals(New Long() { 1, 5, 5 }, arr.shape())
		End Sub

		Private Function getRecord(ByVal length As Integer) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			For i As Integer = 0 To length - 1
				ret.Add(New DoubleWritable(1.0))
			Next i
			Return ret
		End Function
	End Class

End Namespace