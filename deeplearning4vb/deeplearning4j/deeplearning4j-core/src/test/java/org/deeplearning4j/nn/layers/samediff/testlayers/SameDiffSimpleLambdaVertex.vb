Imports System
Imports GraphVertex = org.deeplearning4j.nn.conf.graph.GraphVertex
Imports SameDiffLambdaVertex = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLambdaVertex
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff

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

Namespace org.deeplearning4j.nn.layers.samediff.testlayers

	<Serializable>
	Public Class SameDiffSimpleLambdaVertex
		Inherits SameDiffLambdaVertex

		Public Overrides Function defineVertex(ByVal sameDiff As SameDiff, ByVal inputs As VertexInputs) As SDVariable
			Dim in1 As SDVariable = inputs.getInput(0)
			Dim in2 As SDVariable = inputs.getInput(1)
			Dim ret As SDVariable = in1.mul(in2)
			Return ret
		End Function
	End Class

End Namespace