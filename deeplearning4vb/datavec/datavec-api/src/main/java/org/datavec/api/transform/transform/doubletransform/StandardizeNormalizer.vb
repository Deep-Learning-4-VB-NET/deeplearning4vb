﻿Imports System
Imports Data = lombok.Data
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports Writable = org.datavec.api.writable.Writable
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.transform.doubletransform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class StandardizeNormalizer extends BaseDoubleTransform
	<Serializable>
	Public Class StandardizeNormalizer
		Inherits BaseDoubleTransform

		Protected Friend ReadOnly mean As Double
		Protected Friend ReadOnly stdev As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StandardizeNormalizer(@JsonProperty("columnName") String columnName, @JsonProperty("mean") double mean, @JsonProperty("stdev") double stdev)
		Public Sub New(ByVal columnName As String, ByVal mean As Double, ByVal stdev As Double)
			MyBase.New(columnName)
			Me.mean = mean
			Me.stdev = stdev
		End Sub


		Public Overrides Function map(ByVal writable As Writable) As Writable
			Dim val As Double = writable.toDouble()
			Return New DoubleWritable((val - mean) / stdev)
		End Function

		Public Overrides Function ToString() As String
			Return "StandardizeNormalizer(mean=" & mean & ",stdev=" & stdev & ")"
		End Function

		''' <summary>
		''' Transform an object
		''' in to another object
		''' </summary>
		''' <param name="input"> the record to transform </param>
		''' <returns> the transformed writable </returns>
		Public Overrides Function map(ByVal input As Object) As Object
			Dim n As Number = DirectCast(input, Number)
			Dim val As Double = n.doubleValue()
			Return New DoubleWritable((val - mean) / stdev)
		End Function
	End Class

End Namespace