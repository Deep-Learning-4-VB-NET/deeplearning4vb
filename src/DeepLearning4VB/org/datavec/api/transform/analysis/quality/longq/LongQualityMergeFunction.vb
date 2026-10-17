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

Imports System
Imports DeepLearning4VB.org.nd4j.common.function
Imports LongQuality = DeepLearning4VB.org.datavec.api.transform.quality.columns.LongQuality

Namespace org.datavec.api.transform.analysis.quality.longq

    ''' <summary>Merges two quality reports of a long column by adding their counts.</summary>
    <Serializable>
    Public Class LongQualityMergeFunction
        Implements BiFunction(Of LongQuality, LongQuality, LongQuality)

        Public Overridable Function apply(ByVal v1 As LongQuality, ByVal v2 As LongQuality) As LongQuality Implements BiFunction(Of LongQuality, LongQuality, LongQuality).apply
            Return v1.add(v2)
        End Function
    End Class

End Namespace
