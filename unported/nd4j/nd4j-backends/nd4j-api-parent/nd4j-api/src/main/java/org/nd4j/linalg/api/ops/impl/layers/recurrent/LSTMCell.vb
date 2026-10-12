Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports LSTMCellConfiguration = org.nd4j.linalg.api.ops.impl.layers.recurrent.config.LSTMCellConfiguration

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class LSTMCell extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class LSTMCell
		Inherits DynamicCustomOp

		Private configuration As LSTMCellConfiguration

		Public Sub New(ByVal sameDiff As SameDiff, ByVal configuration As LSTMCellConfiguration)
			MyBase.New(Nothing, sameDiff, configuration.args())
			Me.configuration = configuration
			addIArgument(configuration.iArgs())
			addTArgument(configuration.tArgs())

		End Sub

		Public Overrides Function propertiesForFunction() As IDictionary(Of String, Object)
			Return configuration.toProperties()
		End Function

		Public Overrides Function opName() As String
			Return "lstmCell"
		End Function


		Public Overrides Function onnxName() As String
			Return "LSTM"
		End Function

		Public Overrides Function tensorflowName() As String
			Return MyBase.tensorflowName()
		End Function
	End Class

End Namespace