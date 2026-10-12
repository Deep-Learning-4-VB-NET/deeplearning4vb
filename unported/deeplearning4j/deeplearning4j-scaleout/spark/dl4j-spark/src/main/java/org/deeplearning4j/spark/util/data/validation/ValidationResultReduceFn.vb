Imports Function2 = org.apache.spark.api.java.function.Function2
Imports ValidationResult = org.deeplearning4j.spark.util.data.ValidationResult

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

Namespace org.deeplearning4j.spark.util.data.validation

	Public Class ValidationResultReduceFn
		Implements Function2(Of ValidationResult, ValidationResult, ValidationResult)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.spark.util.data.ValidationResult call(org.deeplearning4j.spark.util.data.ValidationResult v1, org.deeplearning4j.spark.util.data.ValidationResult v2) throws Exception
		Public Overrides Function [call](ByVal v1 As ValidationResult, ByVal v2 As ValidationResult) As ValidationResult
			Return v1.add(v2)
		End Function
	End Class

End Namespace