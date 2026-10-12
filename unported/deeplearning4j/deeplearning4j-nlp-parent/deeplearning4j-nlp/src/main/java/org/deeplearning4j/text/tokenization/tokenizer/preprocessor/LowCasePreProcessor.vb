Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess

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

Namespace org.deeplearning4j.text.tokenization.tokenizer.preprocessor

	Public Class LowCasePreProcessor
		Implements TokenPreProcess

		''' <summary>
		''' Pre process a token
		''' </summary>
		''' <param name="token"> the token to pre process </param>
		''' <returns> the preprocessed token </returns>
		Public Overridable Function preProcess(ByVal token As String) As String Implements TokenPreProcess.preProcess
			Return token.ToLower()
		End Function
	End Class

End Namespace