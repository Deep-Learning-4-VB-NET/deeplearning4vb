Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ColumnType = org.datavec.api.transform.ColumnType

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

Namespace org.datavec.api.transform.analysis.columns

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data @NoArgsConstructor public class BytesAnalysis implements ColumnAnalysis
	<Serializable>
	Public Class BytesAnalysis
		Implements ColumnAnalysis

'JAVA TO VB CONVERTER NOTE: The field countTotal was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private countTotal_Conflict As Long
		Private countNull As Long
		Private countZeroLength As Long
		Private minNumBytes As Integer
		Private maxNumBytes As Integer

		Public Sub New(ByVal builder As Builder)
			Me.countTotal_Conflict = builder.countTotal_Conflict
			Me.countNull = builder.countNull_Conflict
			Me.countZeroLength = builder.countZeroLength_Conflict
			Me.minNumBytes = builder.minNumBytes_Conflict
			Me.maxNumBytes = builder.maxNumBytes_Conflict
		End Sub


		Public Overrides Function ToString() As String
			Return "BytesAnalysis()"
		End Function


		Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.getCountTotal
			Get
				Return countTotal_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property ColumnType As ColumnType Implements ColumnAnalysis.getColumnType
			Get
				Return ColumnType.Bytes
			End Get
		End Property

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field countTotal was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend countTotal_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countNull was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend countNull_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field countZeroLength was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend countZeroLength_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field minNumBytes was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend minNumBytes_Conflict As Integer
'JAVA TO VB CONVERTER NOTE: The field maxNumBytes was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend maxNumBytes_Conflict As Integer

'JAVA TO VB CONVERTER NOTE: The parameter countTotal was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countTotal(ByVal countTotal_Conflict As Long) As Builder
				Me.countTotal_Conflict = countTotal_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countNull was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countNull(ByVal countNull_Conflict As Long) As Builder
				Me.countNull_Conflict = countNull_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter countZeroLength was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function countZeroLength(ByVal countZeroLength_Conflict As Long) As Builder
				Me.countZeroLength_Conflict = countZeroLength_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter minNumBytes was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function minNumBytes(ByVal minNumBytes_Conflict As Integer) As Builder
				Me.minNumBytes_Conflict = minNumBytes_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter maxNumBytes was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maxNumBytes(ByVal maxNumBytes_Conflict As Integer) As Builder
				Me.maxNumBytes_Conflict = maxNumBytes_Conflict
				Return Me
			End Function

			Public Overridable Function build() As BytesAnalysis
				Return New BytesAnalysis(Me)
			End Function
		End Class

	End Class

End Namespace