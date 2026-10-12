Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
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

Namespace org.deeplearning4j.spark.util.data


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @Builder public class ValidationResult implements java.io.Serializable
	<Serializable>
	Public Class ValidationResult
		Private countTotal As Long
		Private countMissingFile As Long
		Private countTotalValid As Long
		Private countTotalInvalid As Long
		Private countLoadingFailure As Long
		Private countMissingFeatures As Long
		Private countMissingLabels As Long
		Private countInvalidFeatures As Long
		Private countInvalidLabels As Long
		Private countInvalidDeleted As Long

		Public Overridable Function add(ByVal o As ValidationResult) As ValidationResult
			If o Is Nothing Then
				Return Me
			End If

			countTotal += o.countTotal
			countMissingFile += o.countMissingFile
			countTotalValid += o.countTotalValid
			countTotalInvalid += o.countTotalInvalid
			countLoadingFailure += o.countLoadingFailure
			countMissingFeatures += o.countMissingFeatures
			countMissingLabels += o.countMissingLabels
			countInvalidFeatures += o.countInvalidFeatures
			countInvalidLabels += o.countInvalidLabels
			countInvalidDeleted += o.countInvalidDeleted

			Return Me
		End Function
	End Class

End Namespace