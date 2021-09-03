﻿Imports Value = lombok.Value
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.space

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Value public class HighLowDiscrete extends DiscreteSpace
	Public Class HighLowDiscrete
		Inherits DiscreteSpace

		'size of the space also defined as the number of different actions
		Friend matrix As INDArray

		Public Sub New(ByVal matrix As INDArray)
			MyBase.New(matrix.rows())
			Me.matrix = matrix
		End Sub

		Public Overridable Overloads Function encode(ByVal a As Integer?) As Object
			Dim m As INDArray = matrix.dup()
			m.put(a.Value - 1, 0, matrix.getDouble(a.Value - 1, 1))
			Return m
		End Function
	End Class

End Namespace