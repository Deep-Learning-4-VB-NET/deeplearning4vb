Imports Function2 = org.apache.spark.api.java.function.Function2
Imports StringAnalysisCounter = org.datavec.api.transform.analysis.counter.StringAnalysisCounter

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

Namespace org.datavec.spark.transform.analysis.string

	Public Class StringAnalysisMergeFunction
		Implements Function2(Of StringAnalysisCounter, StringAnalysisCounter, StringAnalysisCounter)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.transform.analysis.counter.StringAnalysisCounter call(org.datavec.api.transform.analysis.counter.StringAnalysisCounter v1, org.datavec.api.transform.analysis.counter.StringAnalysisCounter v2) throws Exception
		Public Overrides Function [call](ByVal v1 As StringAnalysisCounter, ByVal v2 As StringAnalysisCounter) As StringAnalysisCounter
			Return v1.merge(v2)
		End Function
	End Class

End Namespace