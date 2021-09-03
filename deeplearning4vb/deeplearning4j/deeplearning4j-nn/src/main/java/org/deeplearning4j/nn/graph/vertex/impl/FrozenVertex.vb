Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports TrainingConfig = org.deeplearning4j.nn.api.TrainingConfig
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports DummyConfig = org.deeplearning4j.nn.conf.misc.DummyConfig
Imports BaseWrapperVertex = org.deeplearning4j.nn.graph.vertex.BaseWrapperVertex
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports NoOp = org.nd4j.linalg.learning.config.NoOp

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

Namespace org.deeplearning4j.nn.graph.vertex.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true, exclude = {"config"}) public class FrozenVertex extends org.deeplearning4j.nn.graph.vertex.BaseWrapperVertex
	<Serializable>
	Public Class FrozenVertex
		Inherits BaseWrapperVertex

		Public Sub New(ByVal underlying As GraphVertex)
			MyBase.New(underlying)
		End Sub

'JAVA TO VB CONVERTER NOTE: The field config was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private config_Conflict As DummyConfig

		Public Overrides ReadOnly Property Config As TrainingConfig
			Get
				If config_Conflict Is Nothing Then
					config_Conflict = New DummyConfig(VertexName)
				End If
				Return config_Conflict
			End Get
		End Property
	End Class

End Namespace