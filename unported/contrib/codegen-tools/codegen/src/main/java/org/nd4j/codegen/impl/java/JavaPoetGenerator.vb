Imports StringUtils = org.apache.commons.lang3.StringUtils
Imports Language = org.nd4j.codegen.api.Language
Imports NamespaceOps = org.nd4j.codegen.api.NamespaceOps
Imports Generator = org.nd4j.codegen.api.generator.Generator
Imports GeneratorConfig = org.nd4j.codegen.api.generator.GeneratorConfig

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.codegen.impl.java


	Public Class JavaPoetGenerator
		Implements Generator


		Public Overrides Function language() As Language
			Return Language.JAVA
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void generateNamespaceNd4j(org.nd4j.codegen.api.NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File directory, String className) throws java.io.IOException
		Public Overrides Sub generateNamespaceNd4j(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal directory As File, ByVal className As String)
			Nd4jNamespaceGenerator.generate([namespace], config, directory, className, "org.nd4j.linalg.factory", StringUtils.EMPTY)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void generateNamespaceSameDiff(org.nd4j.codegen.api.NamespaceOps namespace, org.nd4j.codegen.api.generator.GeneratorConfig config, java.io.File directory, String className) throws java.io.IOException
		Public Overrides Sub generateNamespaceSameDiff(ByVal [namespace] As NamespaceOps, ByVal config As GeneratorConfig, ByVal directory As File, ByVal className As String)
			'throw new UnsupportedOperationException("Not yet implemented");
			Nd4jNamespaceGenerator.generate([namespace], config, directory, className, "org.nd4j.autodiff.samediff", StringUtils.EMPTY)
		End Sub
	End Class

End Namespace