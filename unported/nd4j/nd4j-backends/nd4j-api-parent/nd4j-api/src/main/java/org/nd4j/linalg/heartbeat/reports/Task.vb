Imports System.Text
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor

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

Namespace org.nd4j.linalg.heartbeat.reports

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor public class Task
	Public Class Task
		Public Enum NetworkType
			MultilayerNetwork
			ComputationalGraph
			DenseNetwork
		End Enum

		Public Enum ArchitectureType
			CONVOLUTION
			RECURRENT
			RBM
			WORDVECTORS
			UNKNOWN
		End Enum

		Private networkType As NetworkType
		Private architectureType As ArchitectureType

		Private numFeatures As Integer
		Private numLabels As Integer
		Private numSamples As Integer

		Public Overridable Function toCompactString() As String
			Dim builder As New StringBuilder()

			builder.Append("F: ").Append(numFeatures).Append("/")
			builder.Append("L: ").Append(numLabels).Append("/")
			builder.Append("S: ").Append(numSamples).Append(" ")

			Return builder.ToString()
		End Function
	End Class

End Namespace