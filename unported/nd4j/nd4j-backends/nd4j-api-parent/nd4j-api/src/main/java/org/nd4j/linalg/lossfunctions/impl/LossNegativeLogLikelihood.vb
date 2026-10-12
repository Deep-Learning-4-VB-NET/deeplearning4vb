Imports System
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

Namespace org.nd4j.linalg.lossfunctions.impl

	<Serializable>
	Public Class LossNegativeLogLikelihood
		Inherits LossMCXENT

		Public Sub New()
		End Sub

		Public Sub New(ByVal weights As INDArray)
			MyBase.New(weights)
		End Sub

		''' <summary>
		''' The opName of this function
		''' 
		''' @return
		''' </summary>
		Public Overrides Function name() As String
			Return ToString()
		End Function

		Public Overrides Function ToString() As String
			Return "LossNegativeLogLikelihood()"
		End Function
	End Class

End Namespace