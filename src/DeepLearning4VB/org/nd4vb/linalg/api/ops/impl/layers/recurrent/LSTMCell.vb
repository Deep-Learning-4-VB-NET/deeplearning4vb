Imports System
Imports System.Collections.Generic
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports LSTMCellConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.layers.recurrent.config.LSTMCellConfiguration
#If SAMEDIFF Then
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.layers.recurrent

    ''' <summary>The custom op <c>lstmCell</c> (Java <c>org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMCell</c>).</summary>
    Public Class LSTMCell
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private configuration As LSTMCellConfiguration

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal configuration As LSTMCellConfiguration)
			MyBase.New(Nothing, sameDiff, configuration.args())
			Me.configuration = configuration
			addIArgument(configuration.iArgs())
			addTArgument(configuration.tArgs())

		End Sub
#End If

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
