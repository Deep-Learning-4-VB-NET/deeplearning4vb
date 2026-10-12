Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data

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

Namespace org.deeplearning4j.eval.meta

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Prediction extends org.nd4j.evaluation.meta.Prediction
	Public Class Prediction
		Inherits org.nd4j.evaluation.meta.Prediction

		Public Sub New(ByVal actualClass As Integer, ByVal predictedClass As Integer, ByVal recordMetaData As Object)
			MyBase.New(actualClass, predictedClass, recordMetaData)
		End Sub
	End Class

End Namespace