﻿Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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
Namespace org.nd4j.linalg.api.ops.custom


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class Tri extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Tri
		Inherits DynamicCustomOp

		Private dataType As DataType = DataType.FLOAT

		Public Sub New(ByVal sameDiff As SameDiff, ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
			MyBase.New(sameDiff, New SDVariable(){})
			addIArgument(row,column,diag)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
			MyBase.New(sameDiff, New SDVariable(){})
			addIArgument(row,column,diag)
			addDArgument(dataType)
			Me.dataType = dataType


		End Sub

		Public Sub New(ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
			MyBase.New(New INDArray(){}, Nothing)
			addIArgument(row,column,diag)

		End Sub

		Public Sub New(ByVal dataType As DataType, ByVal row As Integer, ByVal column As Integer, ByVal diag As Integer)
			MyBase.New(New INDArray(){}, Nothing)
			addIArgument(row,column,diag)
			addDArgument(dataType)
			Me.dataType = dataType

		End Sub

		Public Overrides Function opName() As String
			Return "tri"
		End Function


		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)

			Return Collections.singletonList(Me.dataType)

		End Function
	End Class

End Namespace